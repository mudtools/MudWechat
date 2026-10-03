// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels.ExternalContact.Attachment;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 客户联系「上传附件资源」域（ExternalContact 模块）契约守卫：路由表、接口层级与令牌绑定锁定。
/// </summary>
/// <remarks>
/// <para>
/// 形态：官方对三类应用开放完全一致的 1 个端点（multipart/form-data 上传，非 JSON 请求体）收敛于父接口
/// <see cref="IWechatWorkExternalContactAttachmentService"/>；
/// 自建 / 第三方 / 代开发子接口均为零差异端点空标记。
/// </para>
/// </remarks>
public class WechatExternalContactAttachmentContractGuards
{
    /// <summary>
    /// 父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string ParentImplementationClassName = "WechatWorkExternalContactAttachmentService";

    private const string ExternalContactRegistryGroupName = "ExternalContact";

    /// <summary>
    /// 契约守卫 UA1：上传附件资源端点路由与参数形态必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// 官方路由在 <c>/cgi-bin/media/</c> 下（非 <c>/cgi-bin/externalcontact/</c> 前缀）；
    /// media_type / attachment_type 走 Query，文件经 multipart/form-data（字段名 media）上传。
    /// </summary>
    [Fact]
    public void AttachmentEndpoints_ShouldMatchOfficialRoutes()
    {
        var parent = typeof(IWechatWorkExternalContactAttachmentService);
        var method = nameof(IWechatWorkExternalContactAttachmentService.UploadAttachmentAsync);

        var target = parent.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        target.Should().NotBeNull($"{parent.Name}.{method} 必须存在");

        var post = target!.GetCustomAttribute<PostAttribute>();
        post.Should().NotBeNull($"{parent.Name}.{method} 必须声明 [Post] 路由");
        post!.RequestUri.Should().Be("/cgi-bin/media/upload_attachment",
            $"{parent.Name}.{method} 路由必须与官方契约一致");

        var parameters = target.GetParameters();
        parameters.Should().HaveCount(4, "端点签名为 mediaType / attachmentType / formData / cancellationToken");

        parameters[0].GetCustomAttribute<QueryAttribute>()!.Name.Should().Be("media_type",
            "媒体文件类型必须以官方契约的 media_type Query 参数传入");
        parameters[1].GetCustomAttribute<QueryAttribute>()!.Name.Should().Be("attachment_type",
            "附件类型必须以官方契约的 attachment_type Query 参数传入");
        parameters[2].GetCustomAttribute<MultipartFormAttribute>().Should().NotBeNull(
            "multipart 表单参数必须标 [MultipartForm]：生成器 3.0.1 对 [FormContent] 发射的 " +
            "GetFormDataContentAsync 调用在运行时接口上不存在（CS1061），[MultipartForm] 才走 IFormContent.ToHttpContentAsync 通路");
        parameters[2].ParameterType.Should().Be(typeof(IFormContent),
            "multipart 表单参数类型必须为组件 IFormContent（AOT 安全的声明式上传通路）");
    }

    /// <summary>
    /// 契约守卫 UA2：接口层级与生成器注册形态——端点收敛父接口（IsAbstract），
    /// 自建 / 第三方 / 代开发三个子接口均为零差异端点空标记（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void AttachmentInterfaceHierarchy_ShouldConvergeOnAbstractParentWithExternalContactRegistry()
    {
        var parent = typeof(IWechatWorkExternalContactAttachmentService);
        var children = new[]
        {
            typeof(IWechatWorkInternalExternalContactAttachmentService),
            typeof(IWechatWorkThirdPartyExternalContactAttachmentService),
            typeof(IWechatWorkProviderExternalContactAttachmentService),
        };

        foreach (var child in children)
        {
            child.Should().BeAssignableTo(parent, $"{child.Name} 必须继承公共父接口 {parent.Name}");
        }

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");

        foreach (var child in children)
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(ExternalContactRegistryGroupName,
                $"{child.Name} 必须挂 {ExternalContactRegistryGroupName} 注册组" +
                $"（与既有的客户联系各域共用 Add{ExternalContactRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(ParentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");
        }

        // 三类应用开放面完全一致：任何子接口出现差异端点均为能力漂移，须先核对官方文档再落位并同批调整 UA1/UA2。
        foreach (var child in children)
        {
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty($"{child.Name} 为空标记：公共端点全部声明于父接口");
        }
    }

    /// <summary>
    /// 契约守卫 UA3：令牌绑定——四接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void AttachmentTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkExternalContactAttachmentService),
            typeof(IWechatWorkInternalExternalContactAttachmentService),
            typeof(IWechatWorkThirdPartyExternalContactAttachmentService),
            typeof(IWechatWorkProviderExternalContactAttachmentService),
        };

        foreach (var iface in interfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（按应用上下文/scope 路由）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    /// <summary>
    /// 契约守卫 UA4：上传附件资源域的响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void AttachmentDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = AttachmentJsonContext.Default;

        context.GetTypeInfo(typeof(UploadAttachmentResponse)).Should().NotBeNull(
            "UploadAttachmentResponse 是上传附件资源域契约面类型，必须登记进 AttachmentJsonContext（AOT 源生成）");
    }
}
