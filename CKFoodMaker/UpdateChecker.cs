using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

//todo 自動アップデート通知の追加
//todo 散らばったFormの統合
//todo 機能単位のUserControl化

namespace CKFoodMaker
{
    public static class UpdateChecker
    {
        private const string _repoOwner = "KujoYuki";
        private const string _repoName = "CoreKeeperFoodEditor";
        private const string _currentVersion = "ver1.2.2";
        private const string _uri = $"https://api.github.com/repos/{_repoOwner}/{_repoName}/releases/latest";

        public static async Task<string> CheckLatestVersionAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_uri);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("CKFoodMaker", "1.0"));

                var response = await client.GetAsync(_uri, HttpCompletionOption.ResponseHeadersRead);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var latestRelease = JsonNode.Parse(json)!.AsObject();
                    var latestVersion = latestRelease.First().Value!["name"]?.GetValue<string>();

                    if (latestVersion != null && latestVersion != _currentVersion)
                    {
                        return $"新しいバージョン {latestVersion} が利用可能です。";
                    }
                    else
                    {
                        return "最新バージョンを使用しています。";
                    }
                }
                else
                {
                    return $"バージョンチェックに失敗しました。{response.StatusCode}";
                }
            }
        }
    }
}
