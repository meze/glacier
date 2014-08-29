using Aws;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Ninject
{
    public class AppModule : NinjectModule
    {
        public override void Load()
        {
            Bind<Upload>().To<Upload>();
            Bind<Glacier>().To<Glacier>();
        }
    }

    public class NinjectServiceLocator
    {
        private readonly IKernel kernel;

        public NinjectServiceLocator()
        {
            kernel = new StandardKernel(new AppModule());
        }

        public Upload Upload
        {
            get { return kernel.Get<Upload>(); }
        }
    }
}
