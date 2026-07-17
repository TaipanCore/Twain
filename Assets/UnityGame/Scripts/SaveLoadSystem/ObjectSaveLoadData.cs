using System;
using Object = System.Object;

public class ObjectSaveLoadData
{
    public String dataObjectId { get; private set; }
    public Object[] data { get; private set; }
    
    public ObjectSaveLoadData(String dataObjectId, Object[] data)
    {
        this.dataObjectId = dataObjectId;
        this.data = data;
    }
}
