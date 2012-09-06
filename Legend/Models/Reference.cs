namespace Legend.Models
{
    public class Reference<T> where T : IGameObject
    {
        public string Id { get; set; }

        public static implicit operator Reference<T>(T doc)
        {
            return new Reference<T>
            {
                Id = doc.Id,
            };
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Reference<T>);
        }

        public bool Equals(Reference<T> obj)
        {
            return obj != null && obj.Id == Id;
        }
    }
}