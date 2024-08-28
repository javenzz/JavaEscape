using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class UserProfile : MonoBehaviour
{
    public static UserProfile Instance;

    public static UnityEvent<ProfileData> OnProfileDataUpdated = new UnityEvent<ProfileData>();

    [SerializeField] public ProfileData profileData;


    private void Awake()
    {
        Instance = this;

        if (profileData == null)
        {
            profileData = new ProfileData();

            //Levels
            profileData.level = 1; // Default level value
            profileData.Level_1_Timer = "";

            //User Informations
            //profileData.playerName = ""; // Default name
            //profileData.Student_Section = "";

            //Quizzes
            profileData.QuizScore_1 = ""; // Default quiz score
            
        }
    }

    void OnEnable()
    {
        UserAccountManager.OnSignInSuccess.AddListener(SignIn);

        UserAccountManager.OnUserDataRecieved.AddListener(UserDataRecieved);
    }

    void OnDisable()
    {
        UserAccountManager.OnSignInSuccess.RemoveListener(SignIn);

        UserAccountManager.OnUserDataRecieved.RemoveListener(UserDataRecieved);
    }

    void SignIn()
    {
        GetUserData();
    }

    [ContextMenu("Get Profile Data")]
    void GetUserData()
    {
        UserAccountManager.Instance.GetUserData("ProfileData");
    }

    void UserDataRecieved(string key, string value)
    {
        if (key == "ProfileData")
        {
            if (!string.IsNullOrEmpty(value))
            {
                profileData = JsonUtility.FromJson<ProfileData>(value);
            }
            else
            {
                // If the value is empty, ensure profileData is initialized
                profileData = new ProfileData();
            }
            OnProfileDataUpdated.Invoke(profileData);
        }
    }

    [ContextMenu("Set Profile Data")]
    void SetUserData(UnityAction OnSucess = null)
    {
        UserAccountManager.Instance.SetUserData("ProfileData", JsonUtility.ToJson(profileData), OnSucess);
    }

    //REFERENCE FOR LEVEL PROGRESSION NI JAVEN
    public void AddLevel()
    {
        profileData.level += 1;

        Door timerLevel = FindAnyObjectByType<Door>();

        // Convert finalElapsedTime (which is in seconds) to TimeSpan
        TimeSpan timeSpan = TimeSpan.FromSeconds(timerLevel.finalElapsedTime);

        // Format the TimeSpan into a string with minutes and seconds
        string timerLevelSave = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);

        // Save the formatted time string into profileData
        profileData.Level_1_Timer = timerLevelSave;

        SetUserData();
        GetUserData();
    }

    public void SetQuizScore()
    {
        QuizManager quizManager = FindObjectOfType<QuizManager>();
        if (quizManager != null)
        {
            if (profileData.QuizScore_1 != null)
            {
                profileData.QuizScore_1 = quizManager.score.ToString() + "/10";

                SetUserData(GetUserData);
            }
            else
            {
                Debug.Log("You already Completed the Quiz");
            }
        }
    }

    public void SetPlayerName(string displayName)
    {
        // Check if the display name is not null or empty
        if (!string.IsNullOrEmpty(displayName))
        {
            profileData.playerName = displayName;

            // Try to set the display name first
            UserAccountManager.Instance.SetDisplayName(displayName, displayNameSuccess =>
            {
                if (displayNameSuccess)
                {
                    // If setting the display name succeeds, set the user data
                    SetUserData(GetUserData);
                    Debug.Log("Player name and user data set successfully.");
                }
                else
                {
                    Debug.LogError("Failed to set display name. User data was not set.");
                }
            });
        }
        else
        {
            Debug.LogWarning("Display name is empty. Please enter a valid name.");
        }
    }


    public void SetPlayerSection(string playerSection)
    {
        // Check if the player section is not null or empty
        if (!string.IsNullOrEmpty(playerSection))
        {
            profileData.Student_Section = playerSection;
            SetUserData(GetUserData);
        }
        else
        {
            Debug.LogWarning("Player section is empty. Please enter a valid section.");
        }
    }

}

[System.Serializable]
public class ProfileData
{
    public string playerName;
    public string Student_Section;
    public int level;
    public string QuizScore_1;
    public string Level_1_Timer;
    
}

/*public class StudentData
{
    public string Student_Name;
    public string Student_Section;
}*/

/*   //SCORING NG QUIZ AAYUSIN PA
     public int QuizScore_2;
     public int QuizScore_3;
     public int QuizScore_4;
     public int QuizScore_5;
     public int QuizScore_6;
     public int QuizScore_7;
     public int QuizScore_8;*/
