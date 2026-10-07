// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils;
using Mud.Wechat.OfficialAccount.DataModels.Menu;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.Menu;

/// <summary>
/// 自定义菜单域 DTO 映射 + **官方 40033 字符集约束的回归用例**。
/// </summary>
public class MpMenuSerializationTests
{
    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMpApp(config =>
        {
            config.AppKey = "mp-menu";
            config.AppId = "wx-menu";
            config.AppSecret = "s-menu";
        });
        services.AddMpServices(builder => builder.AddMenuApi());
        return services.BuildServiceProvider();
    }

    /// <summary>
    /// M1（**关键回归**）：中文菜单名经 SDK 序列化管线<b>不得</b>被转义为 <c>\uXXXX</c>。
    /// </summary>
    /// <remarks>
    /// 官方「创建自定义菜单 / 创建个性化菜单」错误码 <c>40033</c> 原文明确：请求<b>不能包含 <c>\uxxxx</c>
    /// 格式的字符</b>，否则创建失败。而 .NET 源生成 JSON 上下文默认编码器会把非 ASCII 全量转义 ⇒
    /// 若不修复，中文菜单名在真实环境<b>必然</b>创建失败（本用例是该修复的反证：修复前必红）。
    /// </remarks>
    [Fact]
    public async Task MenuRequestBody_ShouldNotEscapeNonAsciiCharacters()
    {
        using var provider = BuildProvider();
        var serializer = provider.GetRequiredService<IHttpContentSerializer>();

        var request = new MpCreateMenuRequest
        {
            Button = new()
            {
                new MpMenuButton { Type = MpMenuButtonTypes.Click, Name = "今日歌曲", Key = "V1001_TODAY_MUSIC" },
            },
        };

        using var content = serializer.ToHttpContent(request);
        content.Should().NotBeNull();

        var json = await content!.ReadAsStringAsync();

        json.Should().NotContain("\\u",
            "官方 40033 明确拒绝 \\uxxxx 形式字符 ⇒ 序列化编码器必须放宽（UnsafeRelaxedJsonEscaping）");
        json.Should().Contain("今日歌曲", "中文菜单名必须以 UTF-8 原文下发");
    }

    /// <summary>M2：创建菜单请求体形态（<c>button</c> 数组 + 二级菜单 <c>sub_button</c>）。</summary>
    [Fact]
    public void CreateMenuRequest_ShouldSerializeToOfficialShape()
    {
        var request = new MpCreateMenuRequest
        {
            Button = new()
            {
                new MpMenuButton { Type = MpMenuButtonTypes.View, Name = "官网", Url = "https://example.com" },
                new MpMenuButton
                {
                    Name = "菜单",
                    SubButton = new()
                    {
                        new MpMenuButton { Type = MpMenuButtonTypes.Click, Name = "赞一下", Key = "V1001_GOOD" },
                    },
                },
            },
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(
            request, MenuJsonContext.Default.MpCreateMenuRequest));
        var buttons = document.RootElement.GetProperty("button");

        buttons.GetArrayLength().Should().Be(2);
        buttons[0].GetProperty("type").GetString().Should().Be("view");
        buttons[0].GetProperty("url").GetString().Should().Be("https://example.com");
        buttons[1].GetProperty("sub_button").ValueKind.Should().Be(JsonValueKind.Array,
            "API 菜单形态的 sub_button 必须为数组（与 selfmenu 的对象形态不同）");
        buttons[1].GetProperty("sub_button").GetArrayLength().Should().Be(1);
        buttons[1].GetProperty("sub_button")[0].GetProperty("key").GetString().Should().Be("V1001_GOOD");
    }

    /// <summary>M3：获取菜单响应（默认菜单 + 个性化菜单含 <c>matchrule</c>）。</summary>
    [Fact]
    public void GetMenuResponse_ShouldParseDefaultAndConditionalMenus()
    {
        const string json = """
            {"menu":{"button":[{"type":"click","name":"今日歌曲","key":"V1001_TODAY_MUSIC"}]},
             "conditionalmenu":[{"button":[{"type":"view","name":"VIP","url":"https://example.com/vip"}],
               "matchrule":{"tag_id":"2","client_platform_type":"2"}}]}
            """;

        var response = JsonSerializer.Deserialize(json, MenuJsonContext.Default.MpGetMenuResponse);

        response!.Menu!.Button.Should().HaveCount(1);
        response.Menu.Button![0].Key.Should().Be("V1001_TODAY_MUSIC");
        response.ConditionalMenu.Should().HaveCount(1);
        response.ConditionalMenu![0].MatchRule!.TagId.Should().Be("2");
        response.ConditionalMenu[0].MatchRule!.ClientPlatformType.Should().Be("2");
    }

    /// <summary>M4：创建个性化菜单响应（<c>menuid</c> 为<b>字符串</b>）。</summary>
    [Fact]
    public void AddConditionalMenuResponse_ShouldParseMenuId()
    {
        const string json = """{"menuid":"208379533"}""";

        var response = JsonSerializer.Deserialize(json, MenuJsonContext.Default.MpAddConditionalMenuResponse);

        response!.MenuId.Should().Be("208379533", "官方示例为字符串形式，勿改为数值");
        response.IsSuccess.Should().BeTrue();
    }

    /// <summary>M5：个性化菜单请求体（<c>button</c> + <c>matchrule</c>）序列化。</summary>
    [Fact]
    public void ConditionalMenu_ShouldSerializeBothSections()
    {
        var menu = new MpConditionalMenu
        {
            Button = new() { new MpMenuButton { Type = MpMenuButtonTypes.View, Name = "VIP", Url = "https://example.com" } },
            MatchRule = new MpMenuMatchRule
            {
                TagId = "2",
                ClientPlatformType = MpMenuClientPlatformTypes.Android,
            },
        };

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(menu, MenuJsonContext.Default.MpConditionalMenu));

        document.RootElement.GetProperty("button").GetArrayLength().Should().Be(1);
        var matchRule = document.RootElement.GetProperty("matchrule");
        matchRule.GetProperty("tag_id").GetString().Should().Be("2");
        matchRule.GetProperty("client_platform_type").GetString().Should().Be("2");
    }

    /// <summary>M6：删除个性化菜单请求体（<c>menuid</c>）+ 测试匹配请求体（<c>user_id</c> 可为微信号）。</summary>
    [Fact]
    public void DeleteAndTryMatchRequests_ShouldUseOfficialFieldNames()
    {
        using var deleteDoc = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpDeleteConditionalMenuRequest { MenuId = "208379533" },
            MenuJsonContext.Default.MpDeleteConditionalMenuRequest));
        deleteDoc.RootElement.GetProperty("menuid").GetString().Should().Be("208379533");

        using var tryMatchDoc = JsonDocument.Parse(JsonSerializer.Serialize(
            new MpTryMatchMenuRequest { UserId = "weixin" },
            MenuJsonContext.Default.MpTryMatchMenuRequest));
        tryMatchDoc.RootElement.GetProperty("user_id").GetString().Should().Be("weixin",
            "官方字段说明为「用户 OpenID 或微信号」");
    }

    /// <summary>M7：测试匹配响应（官方字段表<b>不含</b> matchrule / menuid）。</summary>
    [Fact]
    public void TryMatchMenuResponse_ShouldOnlyExposeButtons()
    {
        const string json = """{"button":[{"type":"click","name":"今日歌曲","key":"V1001"}]}""";

        var response = JsonSerializer.Deserialize(json, MenuJsonContext.Default.MpTryMatchMenuResponse);

        response!.Button.Should().HaveCount(1);
        typeof(MpTryMatchMenuResponse).GetProperty("MatchRule").Should().BeNull(
            "官方字段表未提供 matchrule ⇒ SDK 不据其他页「补全」");
        typeof(MpTryMatchMenuResponse).GetProperty("MenuId").Should().BeNull();
    }

    /// <summary>
    /// M8：查询自定义菜单信息（<c>selfmenu_info</c> 形态——<c>sub_button</c> 为<b>对象</b> <c>{list:[…]}</c>，
    /// 与 API 菜单的数组形态不同；<c>news_info</c> 亦为容器对象）。
    /// </summary>
    [Fact]
    public void SelfMenuInfoResponse_ShouldParseObjectShapedSubButton()
    {
        const string json = """
            {"is_menu_open":1,"selfmenu_info":{"button":[
              {"type":"click","name":"今日歌曲","key":"V1001"},
              {"type":"news","name":"图文","value":"MEDIA_ID","news_info":{"list":[
                 {"title":"标题","digest":"摘要","author":"作者","show_cover":1,
                  "cover_url":"https://example.com/c.png","content_url":"https://example.com/a","source_url":""}]}},
              {"name":"二级容器","sub_button":{"list":[{"type":"view","name":"官网","url":"https://example.com"}]}}
            ]}}
            """;

        var response = JsonSerializer.Deserialize(json, MenuJsonContext.Default.MpGetCurrentSelfMenuInfoResponse);

        response!.IsMenuOpen.Should().Be(1);
        response.SelfMenuInfo!.Button.Should().HaveCount(3);
        response.SelfMenuInfo.Button![0].Key.Should().Be("V1001");
        response.SelfMenuInfo.Button[1].NewsInfo!.List.Should().HaveCount(1);
        response.SelfMenuInfo.Button[1].NewsInfo!.List![0].ShowCover.Should().Be(1);
        response.SelfMenuInfo.Button[2].SubButton!.List.Should().HaveCount(1,
            "selfmenu 形态的 sub_button 是对象 {list:[…]}，不是数组");
        response.SelfMenuInfo.Button[2].SubButton!.List![0].Url.Should().Be("https://example.com");
    }
}
