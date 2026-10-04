// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Work.Abstractions.Authentication.Models;
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.AccountId;
using Mud.Wechat.Work.DataModels.Contacts.Batch;
using Mud.Wechat.Work.DataModels.Contacts.ContactRules;
using Mud.Wechat.Work.DataModels.Contacts.Department;
using Mud.Wechat.Work.DataModels.Contacts.Export;
using Mud.Wechat.Work.DataModels.Contacts.Tags;
using Mud.Wechat.Work.DataModels.Contracts.Users;
using Mud.Wechat.Work.DataModels.CorpGroup;
using Mud.Wechat.Work.DataModels.CorpGroup.ChainContacts;
using Mud.Wechat.Work.DataModels.CorpGroup.Rules;
using Mud.Wechat.Work.DataModels.CorpTokenAuthentication;
using Mud.Wechat.Work.DataModels.ExternalContact.Attachment;
using Mud.Wechat.Work.DataModels.ExternalContact.ContactWay;
using Mud.Wechat.Work.DataModels.ExternalContact.Customer;
using Mud.Wechat.Work.DataModels.ExternalContact.CustomerAcquisition;
using Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;
using Mud.Wechat.Work.DataModels.ExternalContact.InterceptRule;
using Mud.Wechat.Work.DataModels.ExternalContact.AcquisitionComponent;
using Mud.Wechat.Work.DataModels.ExternalContact.FollowUser;
using Mud.Wechat.Work.DataModels.ExternalContact.GroupChat;
using Mud.Wechat.Work.DataModels.ExternalContact.JobInheritance;
using Mud.Wechat.Work.DataModels.ExternalContact.Moment;
using Mud.Wechat.Work.DataModels.ExternalContact.ProductAlbum;
using Mud.Wechat.Work.DataModels.ExternalContact.ResignedInheritance;
using Mud.Wechat.Work.DataModels.ExternalContact.ServedContact;
using Mud.Wechat.Work.DataModels.ExternalContact.Statistics;
using Mud.Wechat.Work.DataModels.ExternalContact.Tag;
using Mud.Wechat.Work.DataModels.Identity;
using Mud.Wechat.Work.DataModels.InternalAppAuthentication;
using Mud.Wechat.Work.DataModels.Kf;
using Mud.Wechat.Work.DataModels.Media;
using Mud.Wechat.Work.DataModels.Message;
using Mud.Wechat.Work.DataModels.MsgAudit;
using Mud.Wechat.Work.DataModels.Pay;
using Mud.Wechat.Work.DataModels.ProviderAuthentication;
using Mud.Wechat.Work.DataModels.Security;
using Mud.Wechat.Work.DataModels.School;
using Mud.Wechat.Work.DataModels.School.ClassPay;
using Mud.Wechat.Work.DataModels.School.HealthReport;
using Mud.Wechat.Work.DataModels.School.Living;

namespace Mud.Wechat.Work.Extensions;

/// <summary>
/// 企业微信 AOT JsonContext 合并扩展（对齐 <c>FeishuJsonResolverExtensions</c>）：
/// 把 SDK 数据模型的源生成序列化上下文合并进组件序列化管线。
/// </summary>
public static class WechatJsonResolverExtensions
{
    /// <summary>
    /// 将企业微信的源生成序列化上下文合并进组件
    /// <c>IOptions&lt;JsonSerializerOptions&gt;</c> 的 TypeInfoResolver 管线（NET8+）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <remarks>
    /// <list type="bullet">
    /// <item>全部 41 个 <c>*JsonContext</c>（<c>DataModels</c> 包 <c>Generated/</c> 目录，由
    /// <c>scripts/GenerateJsonContext.ps1</c>（mud-jsonctx）按 <c>[HttpJsonSerializable]</c>
    /// 标注生成，SerializerClassName = DTO 命名空间的域段，每个上下文与其域 DTO
    /// 同命名空间——与 Mud.Feishu.DataModels「每模块一上下文」同构）；</item>
    /// <item><see cref="AuthenticationJsonContext"/>（<c>Abstractions</c> 包）：授权领域模型
    /// （<c>[HttpJsonSerializable]</c> 覆盖要求，见 P0-5）。</item>
    /// </list>
    /// 各上下文键空间不重叠，合并顺序无关（<c>JsonTypeInfoResolver.Combine</c> 按序命中）。
    /// </remarks>
    public static void ConfigureDataModelsResolver(IServiceCollection services)
    {
        var resolver = JsonTypeInfoResolver.Combine(
            CommonJsonContext.Default,
            UsersJsonContext.Default,
            DepartmentJsonContext.Default,
            TagsJsonContext.Default,
            ContactRulesJsonContext.Default,
            BatchJsonContext.Default,
            ExportJsonContext.Default,
            FollowUserJsonContext.Default,
            CustomerJsonContext.Default,
            TagJsonContext.Default,
            JobInheritanceJsonContext.Default,
            ResignedInheritanceJsonContext.Default,
            GroupChatJsonContext.Default,
            ContactWayJsonContext.Default,
            MomentJsonContext.Default,
            CustomerAcquisitionJsonContext.Default,
            AcquisitionComponentJsonContext.Default,
            GroupMsgJsonContext.Default,
            StatisticsJsonContext.Default,
            ProductAlbumJsonContext.Default,
            InterceptRuleJsonContext.Default,
            AttachmentJsonContext.Default,
            ServedContactJsonContext.Default,
            CorpGroupJsonContext.Default,
            ChainContactsJsonContext.Default,
            RulesJsonContext.Default,
            SecurityJsonContext.Default,
            MessageJsonContext.Default,
            KfJsonContext.Default,
            IdentityJsonContext.Default,
            PayJsonContext.Default,
            MsgAuditJsonContext.Default,
            SchoolJsonContext.Default,
            MediaJsonContext.Default,
            HealthReportJsonContext.Default,
            LivingJsonContext.Default,
            ClassPayJsonContext.Default,
            CorpTokenAuthenticationJsonContext.Default,
            InternalAppAuthenticationJsonContext.Default,
            ProviderAuthenticationJsonContext.Default,
            AccountIdJsonContext.Default,
            AuthenticationJsonContext.Default);
        services.AddMudHttpClientJsonContext(resolver);
    }
}
#endif
