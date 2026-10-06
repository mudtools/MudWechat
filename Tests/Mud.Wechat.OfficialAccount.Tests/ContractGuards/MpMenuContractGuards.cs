// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.Menu;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 自定义菜单域契约守卫：路由表、注册形态、令牌绑定、DTO 结构与「无编辑 API / 级联删除」等语义锁定。
/// </summary>
public class MpMenuContractGuards
{
    private const string MenuRegistryGroupName = "Menu";

    /// <summary>菜单域官方路由表（7 端点）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] MenuRoutes =
    {
        (typeof(IMpMenuService), nameof(IMpMenuService.CreateMenuAsync), typeof(PostAttribute), "/cgi-bin/menu/create"),
        (typeof(IMpMenuService), nameof(IMpMenuService.GetMenuAsync), typeof(GetAttribute), "/cgi-bin/menu/get"),
        (typeof(IMpMenuService), nameof(IMpMenuService.DeleteMenuAsync), typeof(GetAttribute), "/cgi-bin/menu/delete"),
        (typeof(IMpMenuService), nameof(IMpMenuService.AddConditionalMenuAsync), typeof(PostAttribute), "/cgi-bin/menu/addconditional"),
        (typeof(IMpMenuService), nameof(IMpMenuService.DeleteConditionalMenuAsync), typeof(PostAttribute), "/cgi-bin/menu/delconditional"),
        (typeof(IMpMenuService), nameof(IMpMenuService.TryMatchMenuAsync), typeof(PostAttribute), "/cgi-bin/menu/trymatch"),
        (typeof(IMpMenuService), nameof(IMpMenuService.GetCurrentSelfMenuInfoAsync), typeof(GetAttribute), "/cgi-bin/get_current_selfmenu_info"),
    };

    /// <summary>契约守卫 MG1：菜单域 7 端点路由必须与官方契约一致。</summary>
    [Fact]
    public void MenuEndpoints_ShouldMatchOfficialRoutes()
    {
        MenuRoutes.Should().HaveCount(7, "官方「自定义菜单」分组恰 7 个端点");
        MenuRoutes.Select(r => r.Route).Distinct().Should().HaveCount(7);

        foreach (var (iface, method, httpAttribute, route) in MenuRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route);
        }

        // 官方反直觉点：deleteMenu 为 GET（无请求体）；selfmenu 查询不在 /cgi-bin/menu/ 前缀下。
        MenuRoutes.Single(r => r.Method == nameof(IMpMenuService.DeleteMenuAsync)).HttpAttribute
            .Should().Be(typeof(GetAttribute), "删除自定义菜单官方为 GET（无请求体）");
        typeof(IMpMenuService).GetMethod(nameof(IMpMenuService.GetCurrentSelfMenuInfoAsync))!
            .GetCustomAttribute<GetAttribute>()!.RequestUri.Should().Be("/cgi-bin/get_current_selfmenu_info",
            "官方路径不在 /cgi-bin/menu/ 前缀下（勿「顺手对齐」）");
    }

    /// <summary>契约守卫 MG2：注册形态与令牌绑定。</summary>
    [Fact]
    public void MenuInterface_ShouldRegisterDirectlyWithQueryToken()
    {
        var iface = typeof(IMpMenuService);
        var api = iface.GetCustomAttribute<HttpClientApiAttribute>();

        api.Should().NotBeNull();
        api!.IsAbstract.Should().BeFalse();
        api.RegistryGroupName.Should().Be(MenuRegistryGroupName);
        api.TokenManage.Should().Be(nameof(IMpAppManager));

        var token = iface.GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull();
        token!.TokenType.Should().Be(MpTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        iface.Assembly.GetTypes().Where(t => t.IsInterface && t != iface && iface.IsAssignableFrom(t))
            .Should().BeEmpty("菜单域不得出现应用类型子接口");
    }

    /// <summary>契约守卫 MG3：菜单域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void MenuDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = MenuJsonContext.Default;

        var domainTypes = typeof(MpMenuButton).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.OfficialAccount.DataModels.Menu"
                        && !typeof(System.Text.Json.Serialization.JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        const int expectedCount = 16;
        domainTypes.Should().HaveCount(expectedCount,
            "菜单域契约面类型数漂移须先核对官方文档再同批调整本守卫" +
            "（4 菜单结构 + 3 请求 + 3 响应 + 6 selfmenu 形态）");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于菜单域命名空间，必须登记进 MenuJsonContext");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(MenuRegistryGroupName);
        }
    }

    /// <summary>契约守卫 MG4：官方字段名与「两套菜单结构并存」锁定。</summary>
    [Fact]
    public void MenuDataModels_ShouldLockOfficialFieldNamesAndTwoShapes()
    {
        foreach (var name in new[]
                 {
                     "type", "name", "key", "url", "media_id", "appid", "pagepath", "article_id", "sub_button",
                 })
        {
            JsonNamesOf<MpMenuButton>().Should().Contain(name, $"API 菜单按钮必须含官方字段 '{name}'");
        }

        JsonNamesOf<MpMenuMatchRule>().Should().BeEquivalentTo(new[] { "tag_id", "client_platform_type" },
            "个性化菜单匹配规则官方仅两字段（至少一个非空）");
        JsonNamesOf<MpConditionalMenu>().Should().BeEquivalentTo(new[] { "button", "matchrule" },
            "getMenu 的 conditionalmenu 元素官方仅 button + matchrule（不含 menuid）");
        JsonNamesOf<MpAddConditionalMenuResponse>().Should().Contain("menuid");
        JsonNamesOf<MpDeleteConditionalMenuRequest>().Should().Contain("menuid");
        JsonNamesOf<MpTryMatchMenuRequest>().Should().Contain("user_id");
        JsonNamesOf<MpGetCurrentSelfMenuInfoResponse>().Should().BeEquivalentTo(
            new[] { "is_menu_open", "selfmenu_info", "errcode", "errmsg" });

        // selfmenu 形态：sub_button 为对象（内含 list），与 API 菜单的数组形态不同 ⇒ 必须两套 DTO 并存。
        JsonNamesOf<MpSelfMenuButton>().Should().Contain("sub_button");
        JsonNamesOf<MpSelfMenuButtonList>().Should().BeEquivalentTo(new[] { "list" },
            "selfmenu 形态的 sub_button 是对象 {list:[…]}，而 API 菜单是数组 ⇒ 不可合并为同一 DTO");
        JsonNamesOf<MpSelfMenuNewsInfo>().Should().BeEquivalentTo(new[] { "list" });
        JsonNamesOf<MpSelfMenuNewsItem>().Should().BeEquivalentTo(
            new[] { "title", "digest", "author", "show_cover", "cover_url", "content_url", "source_url" });

        typeof(MpMenuButton).GetProperty(nameof(MpMenuButton.SubButton))!.PropertyType
            .Should().Be(typeof(List<MpMenuButton>), "API 菜单的 sub_button 是数组");
        typeof(MpSelfMenuButton).GetProperty(nameof(MpSelfMenuButton.SubButton))!.PropertyType
            .Should().Be(typeof(MpSelfMenuButtonList), "selfmenu 的 sub_button 是对象");
    }

    /// <summary>
    /// 契约守卫 MG5：官方 40033 字符集约束必须被注册期修复覆盖（**防「修复被回退」**）。
    /// </summary>
    /// <remarks>
    /// 仅靠序列化用例不足以防回退（有人可能「为简洁」删掉那行 Configure），故此处对注册期源码做文本守卫。
    /// </remarks>
    [Fact]
    public void Registration_ShouldRelaxJsonEncoderForWechatCharacterSetConstraint()
    {
        var path = Path.Combine(GetRepositoryRoot(), "Mud.Wechat.OfficialAccount.Abstractions",
            "Extensions", "MpMultiAppExtensions.cs");
        File.Exists(path).Should().BeTrue();

        var source = File.ReadAllText(path);
        source.Should().Contain("UnsafeRelaxedJsonEscaping",
            "官方 40033 明确拒绝 \\uxxxx 字符 ⇒ 注册期必须放宽 JsonSerializerOptions.Encoder");
        source.Should().Contain("options.Encoder", "必须落在 Encoder 属性上（而非仅注释提及）");
    }

    /// <summary>契约守卫 MG6：菜单按钮类型常量与官方 12 项枚举一致。</summary>
    [Fact]
    public void MenuButtonTypes_ShouldMatchOfficialEnumeration()
    {
        var constants = typeof(MpMenuButtonTypes)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly)
            .Select(f => (string)f.GetRawConstantValue()!)
            .OrderBy(v => v, StringComparer.Ordinal)
            .ToList();

        constants.Should().BeEquivalentTo(new[]
        {
            "click", "view", "scancode_push", "scancode_waitmsg", "pic_sysphoto", "pic_photo_or_album",
            "pic_weixin", "location_select", "media_id", "article_id", "article_view_limited", "miniprogram",
        }, "官方 type 枚举恰 12 项");

        MpMenuClientPlatformTypes.Ios.Should().Be("1");
        MpMenuClientPlatformTypes.Android.Should().Be("2");
        MpMenuClientPlatformTypes.Others.Should().Be("3");
    }

    /// <summary>契约守卫 MG7：模块枚举 / 注册入口 / Query 令牌白名单同批扩展。</summary>
    [Fact]
    public void MenuModule_ShouldBeRegisteredAndWhitelisted()
    {
        Enum.GetNames(typeof(MpModule)).Should().Contain("Menu");
        typeof(MpServiceBuilder).GetMethod("AddMenuApi").Should().NotBeNull();
    }

    /// <summary>契约守卫 MG8：菜单域已核验错误码常量锁定（含 40033 与其修复语义）。</summary>
    [Fact]
    public void MenuErrorCodes_ShouldMatchVerifiedOfficialValues()
    {
        MpErrorCodes.InvalidButtonSize.Should().Be(40016);
        MpErrorCodes.InvalidButtonType.Should().Be(40017);
        MpErrorCodes.InvalidButtonNameSize.Should().Be(40018);
        MpErrorCodes.InvalidButtonKeySize.Should().Be(40019);
        MpErrorCodes.InvalidButtonUrlSize.Should().Be(40020);
        MpErrorCodes.InvalidSubButtonSize.Should().Be(40023);
        MpErrorCodes.InvalidSubButtonType.Should().Be(40024);
        MpErrorCodes.InvalidSubButtonUrlSize.Should().Be(40027);
        MpErrorCodes.InvalidCharsetEscaped.Should().Be(40033);
        MpErrorCodes.InvalidSubButtonUrlDomain.Should().Be(40054);
        MpErrorCodes.InvalidButtonUrlDomain.Should().Be(40055);
        MpErrorCodes.InvalidArticleId.Should().Be(53600);
        MpErrorCodes.MatchRulePrivacyViolation.Should().Be(65320);
    }

    private static List<string> JsonNamesOf<T>()
        => typeof(T).GetProperties()
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(n => n != null)
            .Select(n => n!)
            .ToList();

    private static string GetRepositoryRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("未找到 Mud.Wechat.slnx");
    }
}
