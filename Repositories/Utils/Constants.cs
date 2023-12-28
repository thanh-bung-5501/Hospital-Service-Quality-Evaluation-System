namespace Repositories.Utils
{
    public static class Constants
    {
        // Character
        public const string UPPER_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string LOWER_CHARACTERS = "abcdefghijklmnopqrstuvwxyz";
        public const string NUMERIC_CHARACTERS = "0123456789";
        // complex special case "!@#$%^&*()_-+=[{]};:<>|./?"
        //private static readonly string SPECIAL_CHARACTERS = "!@#$%^&*_-+=";
        // Email
        public const string FROM = "no.reply.hse.site@gmail.com";
        public const string PASSWORD_OLD = "jyse tfqc jdbz lujp";
        public const string PASSWORD = "iych gqmg rzme fpxu";
        public const string HSE_INTERNAL_USER = "https://int.hsenow.site/authentication/signin";
        // Password
        public const int WORK_FACTOR = 13;
        // Sort
        public const string SERVICE_NAME_ASC = "serName_asc";
        public const string SERVICE_NAME_DESC = "serName_desc";
        public const string CREATED_ON_ASC = "createdOn_asc";
        public const string CREATED_ON_DESC = "createdOn_desc";
        public const string MODIFIED_ON_ASC = "modifiedOn_asc";
        public const string MODIFIED_ON_DESC = "modifiedOn_desc";
        public const string POINT_ASC = "point_asc";
        public const string POINT_DESC = "point_desc";
        public const string PATIENT_NAME_ASC = "patientName_asc";
        public const string PATIENT_NAME_DESC = "patientName_desc";
        // Sort user
        public const string EMAIL_ASC = "email_asc";
        public const string EMAIL_DESC = "email_desc";
        public const string FIRST_NAM_ASC = "firstName_asc";
        public const string FIRST_NAME_DESC = "firstName_desc";
        public const string LAST_NAME_ASC = "lastName_asc";
        public const string LAST_NAME_DESC = "lastName_desc";
        public const string DOB_ASC = "dob_asc";
        public const string DOB_DESC = "dob_desc";
        // Sort role
        public const string MCRITERIA_ASC = "mCriteria_asc";
        public const string MCRITERIA_DESC = "mCriteria_desc";
        public const string MSERVICE_ASC = "mService_asc";
        public const string MSERVICE_DESC = "mService_desc";
        public const string MSYSTEM_ASC = "mSystem_asc";
        public const string MSYSTEM_DESC = "mSystem_desc";
        public const string MUSER_ASC = "mUser_asc";
        public const string MUSER_DESC = "mUser_desc";
        // Page
        public const int PAGE_SIZE = 10;
        // Image
        public const long IMAGE_LIMIT_SIZE = 2097152;
        public const string IMAGE_EXTENSIONS = ".jpg|.png";
        public const string ICON_IMAGE_EXTENSIONS = ".png|.ico";
        // Evaluation Option
        public const string STRONGLY_AGREE = "Strongly Agree";
        public const string AGREE = "Agree";
        public const string NEUTRAL = "Neutral";
        public const string DISAGREE = "Disagree";
        public const string STRONGLY_DISAGREE = "Strongly Disagree";

        public const int STRONGLY_AGREE_POINT = 5;
        public const int AGREE_POINT = 4;
        public const int NEUTRAL_POINT = 3;
        public const int DISAGREE_POINT = 2;
        public const int STRONGLY_DISAGREE_POINT = 1;
        // Month Timeline
        public static readonly List<string> MONTH_TIMELINE_LINE_CHART = new List<string>
        {
            "Week 0","Week 1","Week 2", "Week 3", "Week 4"
        };
        public static readonly List<string> MONTH_TIMELINE_BAR_CHART = new List<string>
        {
            "Week 1","Week 2", "Week 3", "Week 4"
        };
        // Roles
        public const string ADMIN = "Admin";
        public const string QAO = "QAO";
        public const string BOM = "BOM";

        public const string SATISFACTION_DEGREE = "Satisfaction degree";
        public const string NEUTRAL_DEGREE = "Neutral degree";
        public const string DISSATISFACTION_DEGREE = "Dissatisfaction degree";
        public const string POINT = "Point";
        public const string NUMBER_OF_SURVEY = "Number of survey";

        public const string SATISFACTION_DEGREE_DESCRIPTION = "Percentage of 'strongly agree' and 'agree’ votes, which the patient has been evaluated.";
        public const string NEUTRAL_DEGREE_DESCRIPTION = "Percentage of 'neutral' votes that the patient has been evaluated.";
        public const string DISSATISFACTION_DEGREE_DESCRIPTION = "Percentage of 'strongly disagree' and 'disagree' votes, which the patient has been evaluated.";
        public const string POINT_DESCRIPTION = "Total points calculated from patients’ surveys,";
        public const string NUMBER_OF_SURVEY_DESCRIPTION = "Total number of surveys.";
    }
}
