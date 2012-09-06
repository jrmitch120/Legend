<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
    <link rel="stylesheet" type="text/css" href="Css/demo.css" />
</head>
    <body>
        <div class="verticalSpace"></div>
        <div id="main">
            <p>Dialing BBS...</p>
        </div>        
        <table id="inputContainer">
            <tr>
                <td id="prompt"></td>
                <td id="cmdContainer">
                    <input type="text" id="cmd" spellcheck="false" />
                </td>
            </tr>
        </table>
        <div id="bottom"></div>
        <script type="text/javascript" src="Scripts/jquery-1.6.4.js"></script>
        <script type="text/javascript" src="Scripts/jquery.signalR-0.5.3.js"></script>
        <script type="text/javascript" src="<%= ResolveClientUrl("~/signalr/hubs") %>"></script>
        <script type="text/javascript">
            $(function() {
                var myHub = $.connection.legendHub;
                
                /***************************************************************************
                * SignalR Hub stuff
                ***************************************************************************/
                myHub.update = function () {
                    alert('contact from server!');
                };
                
                myHub.receive = function (packet) {
                    $.each(packet.Display.Messages, function () {
                        displayText('main', this.Message, this.Type);
                        //displayText('main', this.Message);
                    });
                };
                
                myHub.disconnect = function (msg) {
                    displayText('main', msg);
                    $('#inputContainer').toggle();
                    $.connection.hub.stop();
                    
                };

                $.connection.hub.start()
                    .done(function () {
                        displayText('main', 'Connected!');
                        login();
                    })
                    .fail(function () {
                        displayText('main','Could not Connect!');
                    });
                
                $.connection.hub.error(function (err) {
                    alert('Got a hub error: ' + err);
                });
            
                /***************************************************************************
                * Function: displayText
                * Desc: Output text to the screen in an old fashinged BBS style.
                ***************************************************************************/
                function displayText(id, text, messageType) {
                    messageType = (typeof messageType === "undefined") ? "" : messageType;
                    text = text.replace(' ', '\u00a0');
                    $('#prompt').toggle();

                    var newContainer = document.createElement('div');
                    newContainer.className = messageType;
                    var node = document.createTextNode(''),
                        i = 0,
                        chars = 20;

                    newContainer.appendChild(node);
                    document.getElementById(id).appendChild(newContainer);
                    
                    (function add() {
                        node.data += text.substr(i, chars);
                        i += chars;
                        if (i < text.length)
                            setTimeout(add, 100 / 6);
                        else
                            $('#prompt').toggle();
                        repositionWindow();
                    })();
                }
                
                /***************************************************************************
                * Function: setPrompt
                * Desc: Set the user prompt to something 
                ***************************************************************************/
                function setPrompt(prompt) {
                    $('#prompt').html(prompt);
                }

                function login() {
                    displayText('main', '');
                    setPrompt('Thou art?:&nbsp;');
                    registerLoginHandler();
                }
                
                function registerCmdHandler() {
                    $('#cmd').unbind('keyup');
                    $('#cmd').keyup(function (event) {
                        if (event.keyCode == 13) {
                            var cmd = $(this).val().trim();
                            if (cmd != '') {
                                $('#main').append('<div>' + $('#prompt').html() + $(this).val() + '</div>');
                                myHub.command(cmd)
                                 .fail(function (err) {
                                     displayText('main', err);
                                 });
                            }

                            $(this).val('');
                        }
                    });
                }
                
                function registerLoginHandler() {
                    $('#cmd').unbind('keyup');
                    $('#cmd').keyup(function (event) {
                        if (event.keyCode == 13) {
                            var userName = $(this).val().trim();
                            $(this).val('');
                            
                            myHub.login(userName)
                                .done(function () {
                                    displayText('main', 'Welcome to the game!');
                                    setPrompt('>');
                                    registerCmdHandler();
                                    myHub.command('look');
                                })
                                .fail(function (err) {
                                    displayText('main', err);
                                    login();
                                });
                        }
                    });
                }
                
                function repositionWindow() {
                    window.scrollTo(0, document.body.scrollHeight);
                }
                
                /***************************************************************************
               * Initinalization stuff.
               ***************************************************************************/
                $('#cmd').focus(); // Initial cursor focus
                $(document).click(function () { $('#cmd').focus(); }); // Force the focus to the CMD prompt
                $(window).resize(function () { repositionWindow(); }); // Resizing the window.  Make sure the command prompt is visible
            });
        </script>
    </body>
</html>
