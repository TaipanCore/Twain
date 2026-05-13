namespace UnityGame.Scripts.Interfaces
{
    public interface IInvulnerableDamageReceiver : IDamageReceiver
    {
        float invulnerableTime { get; set; }
        void GiveInvulnerability();
    }
}