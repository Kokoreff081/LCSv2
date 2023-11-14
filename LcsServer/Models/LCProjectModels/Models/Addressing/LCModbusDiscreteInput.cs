namespace LcsServer.Models.LCProjectModels.Models.Addressing;

//public class LCModbusDiscreteInput : LCAddressObject, ISaveLoad
//{

//    private LCModbusAddressDevice _parent;

//    public LCModbusDiscreteInput(int id, LCModbusAddressDevice parent, ushort index)
//    {
//        Id = id;
//        SaveParentId = parent.Id;
//        _parent = parent;
//        Index = index;
//    }

//    public LCModbusDiscreteInput(int id, int parentId, ushort index)
//    {
//        Id = id;
//        SaveParentId = parentId;
//        Index = index;
//        RadioGroup = radioGroup;
//    }

//    public ushort Index { get; }

//    public byte RadioGroup { get; set; }

//    public override void UnsubscribeAllEvents()
//    {

//    }

//    public override string DisplayName => _parent == null ? Name : $"{_parent.Address}_DI{Index}";


//    public void Save(string projectFolderPath)
//    {
//    }

//    public void Load(List<ISaveLoad> primitives, int indexInPrimitives, string projectFolderPath)
//    {
//        _parent = primitives.OfType<LCModbusAddressDevice>().FirstOrDefault(x => x.Id == ParentId);
//    }

//}