﻿namespace MMTD_Client.Gui
{
    public static class LocalizedStrings
    {
        public static string str_username { get; set; }
        public static string str_password { get; set; }
        public static string str_login { get; set; }
        public static string str_broadcast { get; set; }
        public static string str_social { get; set; }
        public static string str_send { get; set; }
        public static string str_guild { get; set; }
        public static string str_createChannel { get; set; }
        public static string str_create { get; set; }
        public static string str_console { get; set; }

        public static void SetLanguage(string language = "en")
        {
            switch (language)
            {
                case "en": 
                    InitEnglish();
                    break;                   
                case "nl":
                    InitDutch();
                    break;
                default:
                    InitEnglish();
                    break;
            }
        }

        private static void InitEnglish()
        {
            str_username = "Username:";
            str_password = "Password:";
            str_login = "Login";
            str_broadcast = "Broadcast";
            str_social = "Social";
            str_send = "Send";
            str_guild = "Guild";
            str_createChannel = "Create Channel";
            str_create = "Create";
            str_console = "Console";
        }

        private static void InitDutch()
        {
            str_username = "Gebruikersnaam:";
            str_password = "Wachtwoord:";
            str_login = "Inloggen";
            str_broadcast = "Uitzending";
            str_social = "Sociaal";
            str_send = "Verzenden";
            str_guild = "Gilde";
            str_createChannel = "Creëer Kanaal";
            str_create = "Creëer";
            str_console = "Console";
        }
    }
}
