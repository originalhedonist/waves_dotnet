namespace wavegenerator.library
{
    public interface ISettingsSectionProvider<out T>
    {
        T GetSetting();
    }
}