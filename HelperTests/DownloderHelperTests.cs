using Microsoft.VisualStudio.TestTools.UnitTesting;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Tests
{
    [TestClass()]
    public class DownloderHelperTests
    {
        [TestMethod()]
        public async Task DownloadDivideVersionAsyncTestAsync()
        {
            await DownloderHelper.DownloadCompleteVersionAsync("https://www.wenku8.net/modules/article/packshow.php?id=2381&type=txtfull", "UM_distinctid=167d484f8cb2af-0a458c4a3b00c2-8383268-1fa400-167d484f8cd35c; Hm_lvt_d72896ddbf8d27c750e3b365ea2fc902=1545458744; jieqiUserInfo=jieqiUserId%3D136392%2CjieqiUserName%3Da14907%2CjieqiUserGroup%3D3%2CjieqiUserVip%3D0%2CjieqiUserName_un%3Da14907%2CjieqiUserHonor_un%3D%26%23x65B0%3B%26%23x624B%3B%26%23x4E0A%3B%26%23x8DEF%3B%2CjieqiUserGroupName_un%3D%26%23x666E%3B%26%23x901A%3B%26%23x4F1A%3B%26%23x5458%3B%2CjieqiUserLogin%3D1545474508%2CjieqiUserPassword%3D269d1c73d6aa3e859b2168a5442ee6d3; jieqiVisitInfo=jieqiUserLogin%3D1545474508%2CjieqiUserId%3D136392; CNZZDATA5875574=cnzz_eid%3D342914365-1545471076-%26ntime%3D1545479086; CNZZDATA1309966=cnzz_eid%3D1113817109-1545456931-%26ntime%3D1545478619; CNZZDATA1259916661=782748563-1545458618-%7C1545474008; jieqiVisitId=article_articleviews%3D2381; Hm_lpvt_d72896ddbf8d27c750e3b365ea2fc902=1545479389");
        }
    }
}