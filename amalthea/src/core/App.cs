using MvvmCross.IoC;
using MvvmCross.ViewModels;

namespace SoundByte.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            // Register services
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            RegisterCustomAppStart<AppStart>();
        }
    }
}