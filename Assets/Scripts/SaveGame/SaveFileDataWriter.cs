using UnityEngine;
using System;
using System.IO;

public class SaveFileDataWriter
{
    public string saveDataDirectoryPath = "";
    public string saveFileName = "";

    public bool CheckToSeeIfFileExists()
    {
        if(File.Exists(Path.Combine(saveDataDirectoryPath,saveFileName)))
            return true;
        else return false;
    }

    public void DeleteSaveFile()
    {
        File.Delete(Path.Combine(saveDataDirectoryPath,saveFileName));
    }

    public void CreateNewSaveFile(CharacterSaveData cd)
    {
        string saveFilePath = Path.Combine(saveDataDirectoryPath, saveFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(saveFilePath));
            Debug.Log("Creating save file, at save path: " + saveFilePath);

            string dataToStore = JsonUtility.ToJson(cd, true);

            using (FileStream fs = new FileStream(saveFilePath, FileMode.Create))
            { 
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(dataToStore);
                }
            }

        }
        catch (Exception e)
        {
            Debug.Log("Error Trying to save character data, Game NOT SAVED " + saveFilePath + "\n" + e);
        }
        
    }

    public CharacterSaveData LoadSaveFile()
    {
        CharacterSaveData cd = null;

        string loadFilePath = Path.Combine(saveDataDirectoryPath, saveFileName);

        if (File.Exists(loadFilePath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream fs = new FileStream(loadFilePath, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        dataToLoad = sr.ReadToEnd();
                    }
                }

                cd = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load save file " + e);
            }
        }

        return cd;
    }
}
