using UnityEngine;
using System;
using System.Collections;
using SajberSim.Translation;

public static class AS_ErrorHandling
{

    public static bool IsAnError(this string errorString)
    {

        if (errorString.ToLower().Contains("error"))
            return true;
        return false;

    }

    public static string HandleError(this string errorString)
    {

        if (errorString.ToLower().Contains("mysql"))
            return "Error: " + errorString.HandleMySQLError();

        if (errorString.ToLower().Contains("php"))
            return "Error: " + errorString.HandlePHPError();

        return "Error: Unspecified error";

    }

    public static string HandlePHPError(this string errorString)
    {
		if (errorString.ToLower().Contains("could not send"))
			return Translate.Get("phperror");


        return "PHP Error!";
    }

    public static string HandleMySQLError(this string errorString)
    {

        string[] words;

        // DUPLICATE ENTRY
        // MySQL error 1062 (Duplicate entry 'a' for key 'username')
        if (errorString.Contains("1062"))
        {
            words = errorString.ToLower().Split(new string[] { "')" }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 1)
                return "Duplicate entry!";

            words = words[0].Split(new string[] { "for key '" }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 2)
                return "Duplicate entry!";

            return string.Format(Translate.Get("fielddupe"), words[1].ToLower().UppercaseFirst()); //There is an existing account with that {0}.

        }

        if (errorString.ToLower().Contains("could not find the specified email"))
            return Translate.Get("emailnull"); // Could not find an account with that email.

        // GENERIC ERROR HANDLING
        // MySQL error #### (
        words = errorString.ToLower().Split(new string[] { " (" }, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length < 1)
            return "Database error!";

        words = words[0].Split(new string[] { "error " }, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length < 2)
            return "Database error!";

        return "Database error (MySQL Error " + words[1] + ")";

    }
}
