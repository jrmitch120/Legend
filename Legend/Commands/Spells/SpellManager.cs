using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Legend.Models;
using Legend.Services;
using Legend.World;


namespace Legend.Commands.Spells
{
    public class SpellManager
    {
        private readonly Player _player;
        private readonly IWorld _world;
        private readonly INotificationService _notificationService;

        private static readonly Lazy<IList<ISpellCast>> _spells = new Lazy<IList<ISpellCast>>(GetSpells);
        private static Dictionary<string, ISpellCast> _spellCache;

        public SpellManager(Player player, IWorld word, INotificationService notificationService)
        {
            _player = player;
            _world = word;
            _notificationService = notificationService;
        }

        public bool TryHandleSpell(string[] args)
        {
            string spellName = args[0];

            return TryHandleSpell(spellName, args.Skip(1).ToArray());
        }

        private bool TryHandleSpell(string spellName, string[] args)
        {
            ISpellCast spellCast;
            Spell spell;

            if (!TryMatchSpell(spellName, out spellCast))
            {
                throw new InvalidOperationException(
                    String.Format(
                        "...I don't know what kind of spell '{0}' is, but you probably shouldn't mess around with it.",
                        spellName));
            }

            if(!_player.Spells.TryGetValue(spellName, out spell))
                throw new InvalidOperationException(String.Format("...You don't know that spell"));

            // check to see if they have all of the required reagents.
            if (!spell.Reagents.All(x => _player.Items.Any(y => y.Id == x.Id)))
                throw new InvalidOperationException(String.Format("...You feel that the spell is missing something."));

            // Enough Mp?
            if(spell.MpRequired > _player.CurrentMp)
                throw new InvalidOperationException(String.Format("...You're too tired to cast that spell."));

            // High enough level?
            if(spell.MinLevel > _player.Level)
                throw new InvalidOperationException(String.Format("...You're not skilled enough to cast that spell yet."));

            var attrs = Attribute.GetCustomAttributes(spellCast.GetType());
            foreach (var attr in attrs) // Pehaps more attritubes in the future, so for now, iterate through the only 1 posssible ;)
            {
                if (attr is RequiresTargetAttribute && (args.Length == 0 || String.IsNullOrWhiteSpace(args[0])))
                    throw new InvalidOperationException(String.Format("...Who would you like to cast {0} on?", spellName));
            }

            // Good to go, dock them the Mp's
            _player.CurrentMp -= spell.MpRequired;

            var context = new SpellContext
            {
                World = _world,
                NotificationService = _notificationService,
            };

            spellCast.Execute(context, _player, args);

            return true;
        }

        private bool TryMatchSpell(string spellName, out ISpellCast spell)
        {
            if (_spellCache == null)
            {
                var spells = from c in _spells.Value
                               let spellAttribute = c.GetType()
                                                       .GetCustomAttributes(true)
                                                       .OfType<SpellAttribute>()
                                                       .FirstOrDefault()
                               where spellAttribute != null
                               select new
                               {
                                   Name = spellAttribute.SpellName,
                                   Spell = c
                               };

                _spellCache = spells.ToDictionary(c => c.Name, c => c.Spell, StringComparer.OrdinalIgnoreCase);
            }

            return _spellCache.TryGetValue(spellName, out spell);
        }

        private static IList<ISpellCast> GetSpells()
        {
            // Use MEF to locate the content providers in this assembly
            var catalog = new AssemblyCatalog(typeof(SpellManager).Assembly);
            var compositionContainer = new CompositionContainer(catalog);
            return compositionContainer.GetExportedValues<ISpellCast>().ToList();
        }
    }
}