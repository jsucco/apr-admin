using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using menu;
using menu.Controllers;
using System.Web.Mvc;
using System.Threading;
using System.Web.Script.Serialization;

namespace menu.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        [TestMethod]
        public async Task ADLogin()
        {
            bool result = await Helpers.Authenticate.ActiveDirectory("textile\\sso", "uDcgdZqy");

            Assert.IsNotNull(result);

            Assert.AreEqual(true, result);
        }
    }
}
