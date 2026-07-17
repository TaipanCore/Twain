using System;

public interface ISaveLoadObject
{
    public String objectId { get; }
    public void RegisterInSaveLoadSystem();

    public ObjectSaveLoadData PackData();
    public void UnpackData(ObjectSaveLoadData dataToUnpack);
}