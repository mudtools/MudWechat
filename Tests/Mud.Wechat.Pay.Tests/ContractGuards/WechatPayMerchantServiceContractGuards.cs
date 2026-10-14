// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.MerchantService;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P0-7 消费者投诉域契约守卫（<b>11 端点 + 2 手工通道</b>）：通知地址 CRUD + 投诉列表 / 详情 / 协商历史 +
/// 回复 / 完成 / 退款审批 + 即时服务 + 图片上传 / 拉取。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源</b>（2026-10-14 十三页逐字段核验）：通知地址创建 <c>4012458679</c>、更新 <c>4012459282</c>、
/// 查询 <c>4012459014</c>、删除 <c>4012460452</c>；投诉列表 <c>4012533431</c>、详情 <c>4012458106</c>、
/// 协商历史 <c>4012467250</c>；回复 <c>4012467254</c>、完成处理 <c>4012467255</c>、退款审批 <c>4012467256</c>、
/// 即时服务 <c>4017151596</c>（2026 年新页）。逐端点 URL 落在接口 XML 注释（本守卫不复述）。
/// </para>
/// <para>
/// <b>本域是支付线唯一同时含 POST/PUT/GET/DELETE 四动词的域</b>：通知地址四支共用同一路由
/// <c>/v3/merchant-service/complaint-notifications</c>，其中 POST/PUT 请求体相同、GET/POST/PUT 应答相同
/// （守卫 MC-B6），DELETE 为 204 无包体。<b>写支返回形态分裂</b>：回复 / 完成 / 退款审批三支为 204 无包体，
/// 即时服务支为 200 + JSON（回 <c>log_id</c>）。
/// </para>
/// <para>
/// <b>三处「顺手就会写错」</b>：① 退款审批支<b>没有</b> <c>complainted_mchid</c>（与同族其他写支不同）；
/// ② 即时服务支 <c>idempotent_id</c> 必填（官方提供幂等闸，但须宿主生成）；③ 回复支的 <c>mini_program_jump_info</c>
/// 与消息树里的同名对象不是一型（前者三键全必填、后者两键选填且无 <c>text</c>）。
/// </para>
/// </remarks>
public class WechatPayMerchantServiceContractGuards
{
    private const string RegistryGroupName = "MerchantService";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.MerchantService";

    private const string Prefix = "/v3/merchant-service/";

    /// <summary>
    /// MC-B1：11 端点的方法与路由照官方原文；四支 204 无包体（回复 / 完成 / 退款审批 / 删除通知地址），
    /// 其余七支各有应答类型；query 参数只出现在列表 / 协商历史两支。
    /// </summary>
    [Fact]
    public void MerchantServiceEndpoints_ShouldMatchOfficialRoutesAndDeclareNoContentForFourWrites()
    {
        // 通知地址 CRUD（4 端点）
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantService.CreateComplaintNotificationAsync),
            Prefix + "complaint-notifications");
        AssertRoute<PutAttribute>(nameof(IWechatPayMerchantService.UpdateComplaintNotificationAsync),
            Prefix + "complaint-notifications");
        AssertRoute<GetAttribute>(nameof(IWechatPayMerchantService.GetComplaintNotificationAsync),
            Prefix + "complaint-notifications");
        AssertRoute<DeleteAttribute>(nameof(IWechatPayMerchantService.DeleteComplaintNotificationAsync),
            Prefix + "complaint-notifications");

        // 投诉查询（3 端点）
        AssertRoute<GetAttribute>(nameof(IWechatPayMerchantService.ListComplaintsAsync),
            Prefix + "complaints-v2");
        AssertRoute<GetAttribute>(nameof(IWechatPayMerchantService.GetComplaintDetailAsync),
            Prefix + "complaints-v2/{complaintId}");
        AssertRoute<GetAttribute>(nameof(IWechatPayMerchantService.GetNegotiationHistoryAsync),
            Prefix + "complaints-v2/{complaintId}/negotiation-historys");

        // 投诉处理写支（4 端点）
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantService.RespondToComplaintAsync),
            Prefix + "complaints-v2/{complaintId}/response");
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantService.CompleteComplaintAsync),
            Prefix + "complaints-v2/{complaintId}/complete");
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantService.UpdateRefundProgressAsync),
            Prefix + "complaints-v2/{complaintId}/update-refund-progress");
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantService.RespondToImmediateServiceAsync),
            Prefix + "complaints-v2/{complaintId}/response-immediate-service");

        var routes = RoutesOf(typeof(IWechatPayMerchantService));
        routes.Should().HaveCount(11, "本域 11 端点；数量变化须同批更新 MC-B1 与 Tests/ContractBaseline.cs");
        DistinctVerbRoutes().Should().HaveCount(11, "十一条 (方法, 路由) 各不相同：本域没有同方法同路由的多形态分身");

        // 204 面：四支无应答包体（回复 / 完成 / 退款审批 / 删除通知地址）。
        NoContentMethodsOf(typeof(IWechatPayMerchantService))
            .Should().BeEquivalentTo(new[]
            {
                nameof(IWechatPayMerchantService.RespondToComplaintAsync),
                nameof(IWechatPayMerchantService.CompleteComplaintAsync),
                nameof(IWechatPayMerchantService.UpdateRefundProgressAsync),
                nameof(IWechatPayMerchantService.DeleteComplaintNotificationAsync),
            }, "官方仅这三支写操作与 DELETE 成功应答为 204：把即时服务写成裸 Task 即丢掉 log_id");

        // 即时服务支有返回类型（本域唯一返回包体的写支）。
        FindMethod(nameof(IWechatPayMerchantService.RespondToImmediateServiceAsync)).ReturnType
            .Should().Be(typeof(Task<MerchantServiceImmediateServiceResponse>));

        // path 形态：除通知地址四支与列表支外，其余六支都以 complaintId 定位。
        PathKeysOf(nameof(IWechatPayMerchantService.CreateComplaintNotificationAsync)).Should().BeEmpty();
        PathKeysOf(nameof(IWechatPayMerchantService.UpdateComplaintNotificationAsync)).Should().BeEmpty();
        PathKeysOf(nameof(IWechatPayMerchantService.GetComplaintNotificationAsync)).Should().BeEmpty();
        PathKeysOf(nameof(IWechatPayMerchantService.DeleteComplaintNotificationAsync)).Should().BeEmpty();
        PathKeysOf(nameof(IWechatPayMerchantService.ListComplaintsAsync)).Should().BeEmpty();

        foreach (var methodName in new[]
                 {
                     nameof(IWechatPayMerchantService.GetComplaintDetailAsync),
                     nameof(IWechatPayMerchantService.GetNegotiationHistoryAsync),
                     nameof(IWechatPayMerchantService.RespondToComplaintAsync),
                     nameof(IWechatPayMerchantService.CompleteComplaintAsync),
                     nameof(IWechatPayMerchantService.UpdateRefundProgressAsync),
                     nameof(IWechatPayMerchantService.RespondToImmediateServiceAsync),
                 })
        {
            PathKeysOf(methodName).Should().BeEquivalentTo(new[] { "complaintId" },
                $"{methodName}：投诉单号只在 path");
        }

        // query 形态：只有列表支与协商历史支带 query 参数。
        QueryNamesOf(nameof(IWechatPayMerchantService.ListComplaintsAsync))
            .Should().BeEquivalentTo(new[] { "limit", "offset", "begin_date", "end_date", "complainted_mchid" });
        QueryNamesOf(nameof(IWechatPayMerchantService.GetNegotiationHistoryAsync))
            .Should().BeEquivalentTo(new[] { "limit", "offset" });

        // 包体形态：通知地址 POST/PUT 有包体，GET/DELETE 无包体；投诉查询 GET 无包体；写支有包体。
        BodyTypeOf(nameof(IWechatPayMerchantService.CreateComplaintNotificationAsync))
            .Should().Be(typeof(MerchantServiceComplaintNotificationUrlRequest));
        BodyTypeOf(nameof(IWechatPayMerchantService.UpdateComplaintNotificationAsync))
            .Should().Be(typeof(MerchantServiceComplaintNotificationUrlRequest),
                "PUT 与 POST 请求体同一类型（守卫 MC-B6）");
        BodyTypeOf(nameof(IWechatPayMerchantService.RespondToComplaintAsync))
            .Should().Be(typeof(MerchantServiceComplaintResponseRequest));
        BodyTypeOf(nameof(IWechatPayMerchantService.CompleteComplaintAsync))
            .Should().Be(typeof(MerchantServiceComplaintCompleteRequest));
        BodyTypeOf(nameof(IWechatPayMerchantService.UpdateRefundProgressAsync))
            .Should().Be(typeof(MerchantServiceRefundProgressUpdateRequest));
        BodyTypeOf(nameof(IWechatPayMerchantService.RespondToImmediateServiceAsync))
            .Should().Be(typeof(MerchantServiceImmediateServiceRequest));
    }

    /// <summary>
    /// MC-B2：路由全在 <c>/v3/merchant-service/</c> 前缀下 + 两处「官方无此 HTTP 面」零声明
    /// （投诉列表分页游标 / 投诉删除端点），正对照取同批门店域的 DELETE 动词以证扫描机制生效。
    /// </summary>
    [Fact]
    public void MerchantServiceRoutes_ShouldStayOnOwnPrefixAndNotDeclareCursorOrDeleteFaces()
    {
        var routes = RoutesOf(typeof(IWechatPayMerchantService));

        routes.Should().HaveCount(11);
        routes.Count(static r => r.StartsWith(Prefix, StringComparison.Ordinal))
            .Should().Be(11, "本域十一支全在 /v3/merchant-service/ 下，无第二前缀");

        // 零声明①：官方无游标分页 ⇒ 列表支用 offset/limit，不是 cursor。
        QueryNamesOf(nameof(IWechatPayMerchantService.ListComplaintsAsync))
            .Should().NotContain(new[] { "cursor", "next_cursor", "prev_cursor" },
            "官方列表面用 offset/limit 分页：从别域照抄游标即造出官方忽略的查询条件");

        // 零声明②：官方无「删除投诉单」的 HTTP 面。
        typeof(IWechatPayMerchantService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Should().NotContain(static a => a is DeleteAttribute && a.RequestUri!.Contains("complaints-v2", StringComparison.Ordinal),
            "官方十三页内无删除投诉单端点：做成 API 即与官方口径冲突");
        // 正对照：通知地址支确有 DELETE 动词 ⇒ 上面那条零声明不是「扫描把 DELETE 整体漏掉」。
        typeof(IWechatPayMerchantService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Should().Contain(static a => a is DeleteAttribute && a.RequestUri!.EndsWith("notifications", StringComparison.Ordinal),
            "正对照：DELETE 动词在本仓可被基类扫描捕获，投诉单的零 DELETE 是真裁决");
        // 再一层正对照：同批落地的风险域也有 DELETE 动词。
        typeof(IWechatPayMerchantRiskManageService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Should().Contain(static a => a is DeleteAttribute,
            "正对照：DELETE 动词在本仓可被基类扫描捕获，投诉域的零 DELETE 是真裁决");

        Directory.Exists(Path.Combine(SourceProjectDir("Mud.Wechat.Pay"), "Interfaces", RegistryGroupName))
            .Should().BeTrue("正对照：Interfaces/MerchantService/ 确实已落位");
    }

    /// <summary>
    /// MC-B3：官方字段名逐项锁定 + <b>退款审批支无 complainted_mchid</b> + 两型 mini_program_jump_info 分建。
    /// </summary>
    [Fact]
    public void MerchantServiceDtos_ShouldExposeOfficialFieldNamesAndSubtreeDifference()
    {
        // 通知地址请求 / 应答。
        JsonNameShouldBe<MerchantServiceComplaintNotificationUrlRequest>(
            nameof(MerchantServiceComplaintNotificationUrlRequest.Url), "url");
        JsonNameShouldBe<MerchantServiceComplaintNotificationResponse>(
            nameof(MerchantServiceComplaintNotificationResponse.MchId), "mchid");
        JsonNameShouldBe<MerchantServiceComplaintNotificationResponse>(
            nameof(MerchantServiceComplaintNotificationResponse.Url), "url");

        // 回复投诉请求。
        JsonNameShouldBe<MerchantServiceComplaintResponseRequest>(
            nameof(MerchantServiceComplaintResponseRequest.ComplaintedMchId), "complainted_mchid");
        JsonNameShouldBe<MerchantServiceComplaintResponseRequest>(
            nameof(MerchantServiceComplaintResponseRequest.ResponseContent), "response_content");
        JsonNameShouldBe<MerchantServiceComplaintResponseRequest>(
            nameof(MerchantServiceComplaintResponseRequest.ResponseImages), "response_images");
        JsonNameShouldBe<MerchantServiceComplaintResponseRequest>(
            nameof(MerchantServiceComplaintResponseRequest.JumpUrl), "jump_url");
        JsonNameShouldBe<MerchantServiceComplaintResponseRequest>(
            nameof(MerchantServiceComplaintResponseRequest.JumpUrlText), "jump_url_text");
        JsonNameShouldBe<MerchantServiceComplaintResponseRequest>(
            nameof(MerchantServiceComplaintResponseRequest.MiniProgramJumpInfo), "mini_program_jump_info");

        // 回复支的小程序跳转信息（三键全必填）。
        JsonNameShouldBe<MerchantServiceComplaintMiniProgramJumpInfo>(
            nameof(MerchantServiceComplaintMiniProgramJumpInfo.Appid), "appid");
        JsonNameShouldBe<MerchantServiceComplaintMiniProgramJumpInfo>(
            nameof(MerchantServiceComplaintMiniProgramJumpInfo.Path), "path");
        JsonNameShouldBe<MerchantServiceComplaintMiniProgramJumpInfo>(
            nameof(MerchantServiceComplaintMiniProgramJumpInfo.Text), "text");

        // 完成处理请求。
        JsonNameShouldBe<MerchantServiceComplaintCompleteRequest>(
            nameof(MerchantServiceComplaintCompleteRequest.ComplaintedMchId), "complainted_mchid");

        // 退款审批请求（<b>无</b> complainted_mchid）。
        JsonNameShouldBe<MerchantServiceRefundProgressUpdateRequest>(
            nameof(MerchantServiceRefundProgressUpdateRequest.Action), "action");
        JsonNameShouldBe<MerchantServiceRefundProgressUpdateRequest>(
            nameof(MerchantServiceRefundProgressUpdateRequest.LaunchRefundDay), "launch_refund_day");
        JsonNameShouldBe<MerchantServiceRefundProgressUpdateRequest>(
            nameof(MerchantServiceRefundProgressUpdateRequest.RejectReason), "reject_reason");
        JsonNameShouldBe<MerchantServiceRefundProgressUpdateRequest>(
            nameof(MerchantServiceRefundProgressUpdateRequest.RejectMediaList), "reject_media_list");
        JsonNameShouldBe<MerchantServiceRefundProgressUpdateRequest>(
            nameof(MerchantServiceRefundProgressUpdateRequest.Remark), "remark");

        DeclaredJsonNamesOf<MerchantServiceRefundProgressUpdateRequest>()
            .Should().BeEquivalentTo(new[] { "action", "launch_refund_day", "reject_reason", "reject_media_list", "remark" },
            "退款审批支请求体没有 complainted_mchid：与同族其他写支不同（守卫 MC-B3）");

        // 即时服务请求。
        JsonNameShouldBe<MerchantServiceImmediateServiceRequest>(
            nameof(MerchantServiceImmediateServiceRequest.ComplaintedMchId), "complainted_mchid");
        JsonNameShouldBe<MerchantServiceImmediateServiceRequest>(
            nameof(MerchantServiceImmediateServiceRequest.Message), "message");
        JsonNameShouldBe<MerchantServiceImmediateServiceRequest>(
            nameof(MerchantServiceImmediateServiceRequest.IdempotentId), "idempotent_id");

        // 即时服务应答。
        JsonNameShouldBe<MerchantServiceImmediateServiceResponse>(
            nameof(MerchantServiceImmediateServiceResponse.LogId), "log_id");

        // 消息树字段（抽样关键节点）。
        JsonNameShouldBe<MerchantServiceNormalMessage>(
            nameof(MerchantServiceNormalMessage.Blocks), "blocks");
        JsonNameShouldBe<MerchantServiceNormalMessage>(
            nameof(MerchantServiceNormalMessage.SenderIdentity), "sender_identity");
        JsonNameShouldBe<MerchantServiceNormalMessage>(
            nameof(MerchantServiceNormalMessage.CustomData), "custom_data");
        JsonNameShouldBe<MerchantServiceMessageBlock>(
            nameof(MerchantServiceMessageBlock.Type), "type");
        JsonNameShouldBe<MerchantServiceMessageBlock>(
            nameof(MerchantServiceMessageBlock.Text), "text");
        JsonNameShouldBe<MerchantServiceMessageBlock>(
            nameof(MerchantServiceMessageBlock.Image), "image");
        JsonNameShouldBe<MerchantServiceMessageBlock>(
            nameof(MerchantServiceMessageBlock.Link), "link");
        JsonNameShouldBe<MerchantServiceMessageBlock>(
            nameof(MerchantServiceMessageBlock.FaqList), "faq_list");
        JsonNameShouldBe<MerchantServiceMessageBlock>(
            nameof(MerchantServiceMessageBlock.Button), "button");
        JsonNameShouldBe<MerchantServiceMessageBlock>(
            nameof(MerchantServiceMessageBlock.ButtonGroup), "button_group");

        // 按钮组项 vs 单按钮：项无 invalid_info（分建判据之一）。
        DeclaredJsonNamesOf<MerchantServiceButtonItem>()
            .Should().NotContain("invalid_info", "按钮组项无失效配置：与单按钮分建的判据之一");
        DeclaredJsonNamesOf<MerchantServiceButtonBlock>()
            .Should().Contain("invalid_info");

        // 动作里的小程序跳转信息（两键选填、无 text）vs 回复支的三键全必填。
        DeclaredJsonNamesOf<MerchantServiceActionMiniProgramJumpInfo>()
            .Should().BeEquivalentTo(new[] { "appid", "path" },
            "消息树里的小程序跳转信息只有 appid/path 两键且皆选填、没有 text：与回复支的三键全必填型分建（守卫 MC-B3）");
        DeclaredJsonNamesOf<MerchantServiceComplaintMiniProgramJumpInfo>()
            .Should().BeEquivalentTo(new[] { "appid", "path", "text" });

        // 官方误拼字段照页。
        JsonNameShouldBe<MerchantServiceNegotiationHistoryItem>(
            nameof(MerchantServiceNegotiationHistoryItem.UserApplyPlatformServiceReason),
            "user_appy_platform_service_reason");
        JsonNameShouldBe<MerchantServiceNegotiationHistoryItem>(
            nameof(MerchantServiceNegotiationHistoryItem.UserApplyPlatformServiceReasonDescription),
            "user_appy_platform_service_reason_description");

        // 全域名单形态：官方名单全小写下划线，出现大写字母即被驼峰化（防「顺手纠正」）。
        var allNames = DomainTypes().SelectMany(static t => JsonNamesOf(t)).ToArray();
        allNames.Should().NotBeEmpty("参照集为空会让下面的名单形态断言静默恒真");
        allNames.Should().NotContain(static n => n.Any(static c => char.IsUpper(c)),
            "官方名单全小写：出现大写字母即说明有人把 snake_case 驼峰化了");
        allNames.Should().Contain(static n => n.Contains('_'),
            "正对照：本域确有下划线名单，上面的形态断言不是空集恒真");

        foreach (var type in DomainTypes())
        {
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset),
                    $"{type.Name}.{property.Name} 不得用 DateTimeOffset");
                property.PropertyType.Should().NotBe(typeof(DateTime));
            }
        }
    }

    /// <summary>
    /// MC-B4：常量类名册恰 14 个（状态 / 媒体类型 / 问题类型 / 用户标签 / 服务订单状态 / 补充信息类型 /
    /// 退款审批动作 / 消息块类型 / 文本颜色 / 图片样式 / 动作类型 / 按钮布局 / 发送者身份 / 协商操作类型）。
    /// </summary>
    [Fact]
    public void MerchantServiceConstants_ShouldMatchOfficialValueSets()
    {
        ValuesOf(typeof(MerchantServiceComplaintStates)).Should().BeEquivalentTo(new[]
        {
            "PENDING", "PROCESSING", "PROCESSED",
        });

        ValuesOf(typeof(MerchantServiceMediaTypes)).Should().BeEquivalentTo(new[]
        {
            "USER_COMPLAINT_IMAGE", "OPERATION_IMAGE",
        });

        ValuesOf(typeof(MerchantServiceProblemTypes)).Should().BeEquivalentTo(new[]
        {
            "REFUND", "SERVICE_NOT_WORK", "OTHERS",
        });

        ValuesOf(typeof(MerchantServiceUserTags)).Should().BeEquivalentTo(new[]
        {
            "TRUSTED", "HIGH_RISK",
        });

        ValuesOf(typeof(MerchantServiceServiceOrderStates)).Should().BeEquivalentTo(new[]
        {
            "DOING", "REVOKED", "WAITPAY", "DONE",
        });

        ValuesOf(typeof(MerchantServiceAdditionalInfoTypes)).Should().BeEquivalentTo(new[]
        {
            "SHARE_POWER_TYPE",
        });

        ValuesOf(typeof(MerchantServiceRefundProgressActions)).Should().BeEquivalentTo(new[]
        {
            "REJECT", "APPROVE",
        });

        ValuesOf(typeof(MerchantServiceMessageBlockTypes)).Should().BeEquivalentTo(new[]
        {
            "TEXT", "IMAGE", "LINK", "FAQ_LIST", "BUTTON", "BUTTON_GROUP",
        });

        ValuesOf(typeof(MerchantServiceMessageTextColors)).Should().BeEquivalentTo(new[]
        {
            "DEFAULT", "SECONDARY",
        });

        ValuesOf(typeof(MerchantServiceImageStyleTypes)).Should().BeEquivalentTo(new[]
        {
            "IMAGE_STYLE_TYPE_NARROW", "IMAGE_STYLE_TYPE_WIDE",
        });

        ValuesOf(typeof(MerchantServiceActionTypes)).Should().BeEquivalentTo(new[]
        {
            "ACTION_TYPE_SEND_MESSAGE", "ACTION_TYPE_JUMP_URL", "ACTION_TYPE_JUMP_MINI_PROGRAM",
        });

        ValuesOf(typeof(MerchantServiceButtonLayouts)).Should().BeEquivalentTo(new[]
        {
            "LAYOUT_UNKNOWN", "LAYOUT_HORIZONTAL", "LAYOUT_VERTICAL",
        });

        ValuesOf(typeof(MerchantServiceSenderIdentities)).Should().BeEquivalentTo(new[]
        {
            "UNKNOWN", "MANUAL", "MACHINE",
        });

        // 协商操作类型 25 值（含官方误拼）。
        ValuesOf(typeof(MerchantServiceNegotiationOperateTypes)).Should().HaveCount(25);
        ValuesOf(typeof(MerchantServiceNegotiationOperateTypes)).Should().Contain(new[]
        {
            "USER_COMFIRM_COMPLAINT", // 官方误拼：COMFIRM 而非 CONFIRM
            "MERCHANT_REFUSE_RERUND", // 官方误拼：RERUND 而非 REFUND
        }, "官方协商操作类型含两处误拼，SDK 照页不改");

        ConstantClassNames().Should().HaveCount(14,
            "本域常量类恰 14 个：见上方逐一断言");
    }

    /// <summary>
    /// MC-B5：「官方未给 / 不建模」裁决的可机械化部分 —— 本域<b>无加密字段</b>（十三页参数表均无
    /// <c>Wechatpay-Serial</c>）、无官方口径分歧下的客户端校验。
    /// </summary>
    [Fact]
    public void MerchantServiceUnspecifiedOfficialFaces_ShouldRemainUnvalidatedAndUnencrypted()
    {
        // 本域无加密面：十三页参数表内无 Wechatpay-Serial ⇒ DTO 不出现密文 / 加密载体字段。
        var allNames = DomainTypes().SelectMany(static t => JsonNamesOf(t)).ToArray();
        allNames.Should().NotContain(static n => n.Contains("encrypt", StringComparison.Ordinal)
                                                 || n.Contains("serial", StringComparison.Ordinal),
            "投诉面十三页均无加密字段：把其他域的密文字段抄过来即造出官方不认的载荷");

        // 本域不建模投诉删除 ⇒ 类型名不得出现 Delete 分身。
        DomainTypeNames().Should().NotContain(static n => n.Contains("Delete", StringComparison.Ordinal),
            "官方无删除投诉单端点：伪造其 DTO 即造出永远为空的假类型");

        // idempotent_id 官方是 string(128)，不得收窄为数值或 GUID。
        PropertyTypeOf<MerchantServiceImmediateServiceRequest>(nameof(MerchantServiceImmediateServiceRequest.IdempotentId))
            .Should().Be(typeof(string), "idempotent_id 官方是 string(128)：收窄为 GUID 会丢掉非 UUID 形态的值");
    }

    /// <summary>
    /// MC-B6：判错面继承 + 本域「共用裁决」（通知地址 POST/PUT 请求体同一型、GET/POST/PUT 应答同一型、
    /// 列表/详情超集共用一型、按钮组项与单按钮分建、两型 mini_program_jump_info 分建）。
    /// </summary>
    [Fact]
    public void MerchantServiceResponses_ShouldDeriveFromBaseAndFollowSharedSubtreeRulings()
    {
        foreach (var responseType in new[]
                 {
                     typeof(MerchantServiceComplaintNotificationResponse),
                     typeof(MerchantServiceComplaintListResponse),
                     typeof(MerchantServiceComplaintDetail),
                     typeof(MerchantServiceNegotiationHistoryResponse),
                     typeof(MerchantServiceImmediateServiceResponse),
                 })
        {
            responseType.BaseType.Should().Be(typeof(WechatPayResponse),
                $"{responseType.Name} 必须直接继承 WechatPayResponse，否则 4xx 的 code/message 无落点");
        }

        foreach (var requestType in new[]
                 {
                     typeof(MerchantServiceComplaintNotificationUrlRequest),
                     typeof(MerchantServiceComplaintResponseRequest),
                     typeof(MerchantServiceComplaintCompleteRequest),
                     typeof(MerchantServiceRefundProgressUpdateRequest),
                     typeof(MerchantServiceImmediateServiceRequest),
                 })
        {
            typeof(WechatPayResponse).IsAssignableFrom(requestType).Should().BeFalse(
                $"{requestType.Name} 是请求类型，不得挂在应答基类上");
        }

        // 共用①：通知地址 POST/PUT 请求体同一型。
        BodyTypeOf(nameof(IWechatPayMerchantService.CreateComplaintNotificationAsync))
            .Should().Be(BodyTypeOf(nameof(IWechatPayMerchantService.UpdateComplaintNotificationAsync)),
            "POST 与 PUT 请求体同一类型（守卫 MC-B6）");

        // 共用②：通知地址 GET/POST/PUT 应答同一型。
        FindMethod(nameof(IWechatPayMerchantService.GetComplaintNotificationAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayMerchantService.CreateComplaintNotificationAsync)).ReturnType,
            "GET/POST/PUT 应答同一类型（守卫 MC-B6）");
        FindMethod(nameof(IWechatPayMerchantService.UpdateComplaintNotificationAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayMerchantService.GetComplaintNotificationAsync)).ReturnType);

        // 共用③：列表/详情超集共用一型（详情只多 payer_openid 一个响应键，请求侧无差异）。
        // 注意：本域实际是分建两型（ListItem vs Detail），因为详情多了 payer_openid 且官方在列表页明确说不在列表里。
        // 这与「超集共用」的判据相反 —— 这里的差异是「有无该键」而非「必填性不同」，但 SDK 选择分建以便类型安全。

        // 分建①：按钮组项 vs 单按钮（项无 invalid_info）。
        typeof(MerchantServiceButtonItem).Should().NotBe(typeof(MerchantServiceButtonBlock),
            "按钮组项与单按钮分建：项无 invalid_info（守卫 MC-B3）");

        // 分建②：两型 mini_program_jump_info（回复支三键全必填 vs 消息树两键选填无 text）。
        typeof(MerchantServiceComplaintMiniProgramJumpInfo).Should().NotBe(typeof(MerchantServiceActionMiniProgramJumpInfo),
            "回复支的小程序跳转信息与消息树里的同名对象分建：前者三键全必填、后者两键选填且无 text（守卫 MC-B3）");
    }

    /// <summary>
    /// MC-B7：本域 DTO 全量登记进 AOT 上下文 + 三段式注册接线齐备 + 域内零 <c>[Token]</c>。
    /// </summary>
    [Fact]
    public void MerchantServiceDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired()
    {
        var domainTypes = DomainTypes().ToList();

        domainTypes.Should().HaveCountGreaterThan(0, "本域应有若干 DTO 类型");

        foreach (var type in domainTypes)
        {
            MerchantServiceJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 MerchantServiceJsonContext（否则 Native AOT 下无元数据）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        // 常量类不得被标注（上下文类已排除）。
        var allTypeNames = DomainTypeNames();
        allTypeNames.Should().HaveCountGreaterThan(0, "本域类型数应大于 0");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay",
            "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("MerchantServiceJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线：无登记即无元数据）");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 MerchantService 成员（漏挂即 AddModules 静默不注册）");
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddMerchantServiceWebApiHttpClient()",
            "注册委托必须指向源生成方法（编译期已证其存在，此处锁「真的被挂上」）");
        builderSource.Should().Contain("IWechatPayComplaintMediaService",
            "图片通道须以 TryAddSingleton 注册（手工通道，非声明式）");

        var api = typeof(IWechatPayMerchantService)
            .GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api).Should().Be(RegistryGroupName);
        api.GetType().GetProperty("HttpClient")!.GetValue(api)
            .Should().Be(WechatPayHttpClientNames.TypeName,
                "本域必须绑支付专用命名客户端（wechat-pay）");
        typeof(IWechatPayMerchantService)
            .GetCustomAttributes(false)
            .Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
                "支付线凭据为商户 RSA 私钥签名，不存在 access_token（红线 PAY-B1）");
    }

    // ---- helpers -------------------------------------------------------------

    private static void AssertRoute<TAttribute>(string methodName, string route)
        where TAttribute : HttpMethodAttribute
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<TAttribute>();
        attribute.Should().NotBeNull($"{methodName} 必须声明 {typeof(TAttribute).Name}");
        attribute!.RequestUri.Should().Be(route, $"{methodName} 路由必须与官方契约一致");
    }

    private static (string Verb, string Route)[] DistinctVerbRoutes()
        => typeof(IWechatPayMerchantService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>()
                .Select(static a => (a.GetType().Name, a.RequestUri!)))
            .Distinct()
            .ToArray();

    private static MethodInfo FindMethod(string methodName)
        => typeof(IWechatPayMerchantService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayMerchantService.{methodName} 必须存在");

    private static Type BodyTypeOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Single(static p => p.GetCustomAttribute<PathAttribute>() == null
                                 && p.GetCustomAttribute<QueryAttribute>() == null
                                 && p.ParameterType != typeof(CancellationToken))
            .ParameterType;

    private static string[] QueryNamesOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name!)
            .ToArray();

    private static string[] PathKeysOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name!)
            .ToArray();

    private static string[] NoContentMethodsOf(Type serviceType)
        => serviceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(static m => m.ReturnType == typeof(Task))
            .Select(static m => m.Name)
            .ToArray();

    private static string[] RoutesOf(Type serviceType)
        => serviceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!)
            .ToArray();

    private static IEnumerable<Type> DomainTypes()
        => typeof(MerchantServiceComplaintNotificationUrlRequest).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] DomainTypeNames()
        => typeof(MerchantServiceComplaintNotificationUrlRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsPublic
                               && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .Select(static t => t.Name)
            .ToArray();

    private static string[] ConstantClassNames()
        => typeof(MerchantServiceComplaintNotificationUrlRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsAbstract && t.IsSealed)
            .Select(static t => t.Name)
            .ToArray();

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName)
        => JsonNameShouldBe(typeof(T), propertyName, expectedJsonName);

    private static void JsonNameShouldBe(Type type, string propertyName, string expectedJsonName)
    {
        var property = type.GetProperty(propertyName);
        property.Should().NotBeNull($"{type.Name}.{propertyName} 必须存在（官方契约面漂移）");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(expectedJsonName);
    }

    private static Type PropertyTypeOf<T>(string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须存在");
        return property!.PropertyType;
    }

    private static string[] DeclaredJsonNamesOf<T>()
        => typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null)
            .Select(static n => n!)
            .ToArray();

    private static string[] JsonNamesOf(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null)
            .Select(static n => n!)
            .ToArray();

    private static string[] ValuesOf(Type constantsType)
        => constantsType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToArray();

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName)
        => SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(Root, $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();

    private static string Root
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
            {
                dir = dir.Parent;
            }
            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }
}
