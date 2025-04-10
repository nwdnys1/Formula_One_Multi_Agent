using System;

namespace DTO
{
    public class JsonStr
    {
        public static string media_interview_start =
        "{" +
            "\"scene_type\": \"media_interview\", " +
            "\"scene_data\": {" +
                "\"sender\": \"unity\", " +
                "\"receiver\": [\"Journalist\", \"Verstappen\", \"Hamilton\", \"Horner\"], " +
                "\"signal\": \"media_interview_start\"" +
            "}" +
        "}";
        public static string media_interview_chat(string content)
        {
            return
                "{" +
                    "\"scene_type\": \"media_interview\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"Wolff\", " +
                        "\"receiver\": [\"Journalist\"], " +
                        "\"content\": \"" + content + "\", " +
                        "\"signal\": \"chat\"" +
                    "}" +
                "}";
        }

        public static string media_interview_end =
            "{" +
                "\"scene_type\": \"media_interview\", " +
                "\"scene_data\": {" +
                    "\"sender\": \"unity\", " +
                    "\"receiver\": \"Journalist\", " +
                    "\"signal\": \"media_interview_end\"" +
                "}" +
            "}";


        public static string practice_session_start =
                "{" +
                    "\"scene_type\": \"practice_session\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"unity\", " +
                        "\"receiver\": [\"Strategist\", \"Hamilton\"], " +
                        "\"signal\": \"practice_session_start\"" +
                    "}" +
                "}";


        public static string before_meeting_start =
                "{" +
                    "\"scene_type\": \"before_meeting\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"unity\", " +
                        "\"receiver\": [\"Wolff\", \"Hamilton\", \"Mechanic\", \"Strategist\"], " +
                        "\"signal\": \"before_meeting_start\"" +
                    "}" +
                "}";

        public static string meeting_replay =
                "{" +
                    "\"scene_type\": \"before_meeting\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"unity\", " +
                        "\"receiver\": [\"Wolff\", \"Hamilton\", \"Mechanic\", \"Strategist\"], " +
                        "\"signal\": \"meeting_replay\"" +
                    "}" +
                "}";

        public static string meeting_strategy =
                "{" +
                    "\"scene_type\": \"before_meeting\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"unity\", " +
                        "\"receiver\": [\"Wolff\", \"Hamilton\", \"Strategist\"], " +
                        "\"signal\": \"meeting_strategy\"" +
                    "}" +
                "}";


        public static string meeting_attitude =
            "{" +
                "\"scene_type\": \"before_meeting\", " +
                "\"scene_data\": {" +
                    "\"sender\": \"unity\", " +
                    "\"receiver\": [\"Wolff\", \"Hamilton\"], " +
                    "\"signal\": \"meeting_attitude\"" +
                "}" +
            "}";

        public static string meeting_chat(string content, string receiver)
        {
            return
                "{" +
                    "\"scene_type\": \"before_meeting\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"Wolff\", " +
                        "\"receiver\": \"" + receiver + "\", " +
                        "\"content\": \"" + content + "\", " +
                        "\"signal\": \"chat\"" +
                    "}" +
                "}";
        }
        public static string before_meeting_end =
             "{" +
                "\"scene_type\": \"before_meeting\", " +
                "\"scene_data\": {" +
                    "\"sender\": \"unity\", " +
                    "\"receiver\": [\"Wolff\", \"Hamilton\", \"Mechanic\", \"Strategist\"], " +
                    "\"signal\": \"before_meeting_end\"" +
                "}" +
            "}";


        public static string race_start =
            "{" +
                "\"scene_type\": \"race\", " +
                "\"scene_data\": {" +
                    "\"sender\": \"unity\", " +
                    "\"receiver\": [\"Hamilton\", \"Wolff\", \"Strategist\", \"Strategist\"], " +
                    "\"signal\": \"race_start\"" +
               "}" +
            "}";

        public static string weather_update =
                    "{" +
                        "\"scene_type\": \"race\", " +
                        "\"scene_data\": {" +
                            "\"sender\": \"unity\", " +
                            "\"receiver\": \"environment\", " +
                            "\"signal\": \"weather_update\"" +
                        "}" +
                    "}";

        public static string track_conditions_update =
            "{" +
                "\"scene_type\": \"race\", " +
                "\"scene_data\": {" +
                    "\"sender\": \"unity\", " +
                    "\"receiver\": \"environment\", " +
                    "track_conditions\": {" +
                        "\"Grip Level\": 0.5, " +
                        "\"Rubber Particles\": \"Medium\" " +
                    "}" +
                    "\"signal\": \"track_conditions_update\"" +
                "}" +
            "}";


        public static string Grid_Position_upadate(string positions)
        {
            return
                "{" +
                    "\"scene_type\": \"race\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"unity\", " +
                        "\"receiver\": \"environment\", " +
                        "\"Grid_Position\": " + positions + "," +
                        "\"signal\": \"Grid_Position_upadate\"" +
                    "}" +
                "}";
        }


        public static string Lap_Time_upadate(string lap_time)
        {
            return
                "{" +
                    "\"scene_type\": \"race\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"unity\", " +
                        "\"receiver\": \"environment\", " +
                        "\"Lap_Time\": " + lap_time + "," +
                        "\"signal\": \"Lap_Time_upadate\"" +
                    "}" +
                "}";
        }

        public static string Pit_Stop_Count_upadate(string pit_stop_count)
        {
            return
                "{" +
                    "\"scene_type\": \"race\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"unity\", " +
                        "\"receiver\": \"environment\", " +
                        "\"Pit_Stop_Count\": " + pit_stop_count + "," +
                        "\"signal\": \"Pit_Stop_Count_upadate\"" +
                    "}" +
                "}";
        }

        public static string Tyre_Condition_upadate(string tyre_condition)
        {
            return
                "{" +
                    "\"scene_type\": \"race\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"unity\", " +
                        "\"receiver\": \"environment\", " +
                        "\"Tyre_Condition\": " + tyre_condition + "," +
                        "\"signal\": \"Tyre_Condition_upadate\"" +
                    "}" +
                "}";
        }

        public static string race_chat(string content)
        {
            return
                "{" +
                    "\"scene_type\": \"race\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"Wolff\", " +
                        "\"receiver\": [\"Hamilton\", \"Mechanic\", \"Strategist\"], " +
                        "\"content\": \"" + content + "\", " +
                        "\"signal\": \"chat\"" +
                    "}" +
                "}";
        }


        public static string strategy_update(string content, string strategy)
        {
            return
                "{" +
                    "\"scene_type\": \"race\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"Wolff\", " +
                        "\"receiver\": \"Strategist\", " +
                        "\"content\": \"" + content + "\", " +
                        "\"strategy\": \"" + strategy + "\", " +
                        "\"signal\": \"strategy_update\"" +
                    "}" +
                "}";
        }

        public static string attitude_update(string content, string attitude)
        {
            return
                "{" +
                    "\"scene_type\": \"race\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"Wolff\", " +
                        "\"receiver\": \"Hamilton\", " +
                        "\"content\": \"" + content + "\", " +
                        "\"attitude\": \"" + attitude + "\", " +
                        "\"signal\": \"attitude_update\"" +
                    "}" +
                "}";
        }

        public static string accident_occurred(string driver)
        {
            return
                "{" +
                    "\"scene_type\": \"race\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"unity\", " +
                        "\"receiver\": [\"Wolff\", \"Hamilton\", \"Strategist\"], " +
                        "\"signal\": \"accident_occurred\", " +
                        "\"accident_driver\": \"" + driver + "\"" +
                    "}" +
                "}";
        }

        public static string after_meeting_start =
            "{" +
                "\"scene_type\": \"after_meeting\", " +
                "\"scene_data\": {" +
                    "\"sender\": \"unity\", " +
                    "\"receiver\": [\"Wolff\", \"Hamilton\", \"Mechanic\", \"Strategist\"], " +
                    "\"signal\": \"after_meeting_start\"" +
                "}" +
            "}";

        public static string after_meeting_end =
            "{" +
                "\"scene_type\": \"after_meeting\", " +
                "\"scene_data\": {" +
                    "\"sender\": \"unity\", " +
                    "\"receiver\": [\"Wolff\", \"Hamilton\", \"Mechanic\", \"Strategist\"], " +
                    "\"signal\": \"after_meeting_end\"" +
                "}" +
            "}";

    }

    [Serializable]
    public class SceneData
    {
        public string sender;
        public string receiver;
        public string content;
    }

    [Serializable]
    public class RequestData
    {
        public string scene_type;
        public SceneData scene_data;
    }
}