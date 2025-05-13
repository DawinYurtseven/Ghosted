public class DoorFear : FearObjectParent
{
    public TalismanTargetMock TargetMock;
    
    public override void Lock() {
        _locked = !_locked;
        if (specialEffect != null)
        {
            specialEffect.SetActive(true);
        }
    }
    
}
