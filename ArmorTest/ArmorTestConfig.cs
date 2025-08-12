using OnixRuntime.Api.OnixClient;
namespace ArmorTest {
    public partial class ArmorTestConfig : OnixModuleSettingRedirector {

        [Value(true)]
        public partial bool ShowSlotTexture {  get; set; }
    }
}