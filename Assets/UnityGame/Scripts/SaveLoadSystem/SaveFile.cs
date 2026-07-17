using System;
using System.Collections.Generic;

public class SaveFile
{
    public String saveTime;
    public List<ObjectSaveLoadData> objectsData;
    
    public SaveFile(List<ObjectSaveLoadData> data)
    {
        saveTime = DateTime.Now.ToString("g");
        objectsData = data;
    }
}
