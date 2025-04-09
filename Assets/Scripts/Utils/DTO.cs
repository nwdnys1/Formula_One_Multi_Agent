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
                "\"receiver\": [\"����\", \"ά˹����\", \"���ܶ���\", \"����\"], " +
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
                        "\"receiver\": [\"����\"], " +
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
                    "\"receiver\": \"journalist\", " +
                    "\"signal\": \"media_interview_end\"" +
                "}" +
            "}";


        public static string practice_session_start =
                "{" +
                    "\"scene_type\": \"practice_session\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"unity\", " +
                        "\"receiver\": [\"÷�����Ӳ���ʦ\", \"���ܶ���\"], " +
                        "\"signal\": \"practice_session_start\"" +
                    "}" +
                "}";


        public static string before_meeting_start =
                "{" +
                    "\"scene_type\": \"before_meeting\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"unity\", " +
                        "\"receiver\": [\"Wolff\", \"���ܶ���\", \"÷�����ӻ�еʦ\", \"÷�����Ӳ���ʦ\"], " +
                        "\"signal\": \"before_meeting_start\"" +
                    "}" +
                "}";

        public static string meeting_replay =
                "{" +
                    "\"scene_type\": \"before_meeting\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"unity\", " +
                        "\"receiver\": [\"Wolff\", \"���ܶ���\", \"÷�����ӻ�еʦ\", \"÷�����Ӳ���ʦ\"], " +
                        "\"signal\": \"meeting_replay\"" +
                    "}" +
                "}";

        public static string meeting_strategy =
                "{" +
                    "\"scene_type\": \"before_meeting\", " +
                    "\"scene_data\": {" +
                        "\"sender\": \"unity\", " +
                        "\"receiver\": [\"Wolff\", \"���ܶ���\", \"÷�����Ӳ���ʦ\"], " +
                        "\"signal\": \"meeting_strategy\"" +
                    "}" +
                "}";


        public static string meeting_attitude =
            "{" +
                "\"scene_type\": \"before_meeting\", " +
                "\"scene_data\": {" +
                    "\"sender\": \"unity\", " +
                    "\"receiver\": [\"Wolff\", \"���ܶ���\"], " +
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
                    "\"receiver\": [\"Wolff\", \"���ܶ���\", \"÷�����ӻ�еʦ\", \"÷�����Ӳ���ʦ\"], " +
                    "\"signal\": \"before_meeting_end\"" +
                "}" +
            "}";

        public static string race_start = "{\r\n    \"scene_type\": \"race\",\r\n    \"scene_data\": {\r\n        \"sender\": \"unity\",\r\n        \"receiver\": [\"���ܶ���\", \"Wolff\", \"÷�����Ӳ���ʦ\", \"÷�����Ӳ���ʦ\"], \r\n        \"signal\": \"race_start\" \r\n    }\r\n}";

        public static string weather_update = "";

        public static string track_conditions_update = "";

        public static string Grid_Position_upadate = "";

        public static string Lap_Time_upadate = "";

        public static string Pit_Stop_Count_upadate = "";

        public static string Tyre_Condition_upadate = "";

        public static string qwe = "";

        public static string strategy_update = "";

        public static string attitude_update = "";

        public static string overtake_occurred = "";

        public static string after_meeting_start = "";

        public static string after_meeting_end = "";

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