// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.OrgApprovalDemo.Configuration;

/// <summary>
/// 配置面登记（方案 §8.2）：每个可覆盖项都是「一个变量名 + 一个配置键」的成对来源，
/// <b>变量优先、JSON 其次</b>，两处共用 <c>DemoEnv</c> 的同一套占位符判定（占位符 = 未配置）。
/// </summary>
/// <remarks>
/// <para>
/// <b>写闸与回调用凭据刻意不在这里</b>：<c>MUDWECHAT_DEMO_ALLOW_WRITE</c> 是写操作的唯一放行入口
/// （基座契约，见 <c>Demos/README.md</c> §3）；回调 <c>PushToken</c> / <c>EncodingAESKey</c> 是「接收面
/// 验签与解密密钥」，写进可提交的文件等于把安全边界交给版本库。
/// </para>
/// </remarks>
public static class DemoConfig
{
    /// <summary>配置节名。</summary>
    public const string Section = "OrgDemo";

    /// <summary>企业 CorpId 的变量名（不含 <c>MUDWECHAT_DEMO_</c> 前缀）。</summary>
    public const string CorpIdEnv = "WORK_CORP_ID";

    /// <summary>企业 CorpId 的配置键。</summary>
    public const string CorpIdKey = Section + ":CorpId";

    /// <summary>通讯录同步助手 secret 的变量名（读写部门/成员的唯一合法身份）。</summary>
    public const string ContactSecretEnv = "WORK_CONTACT_SECRET";

    /// <summary>通讯录同步助手 secret 的配置键。</summary>
    public const string ContactSecretKey = Section + ":ContactSecret";

    /// <summary>审批应用 secret 的变量名（模板读写与审批单回捞）。</summary>
    public const string ApprovalSecretEnv = "WORK_APPROVAL_SECRET";

    /// <summary>审批应用 secret 的配置键。</summary>
    public const string ApprovalSecretKey = Section + ":ApprovalSecret";

    /// <summary>SQLite 库路径的配置键（变量名见 <c>OrgDbPath.EnvName</c>）。</summary>
    public const string DbPathKey = Section + ":DbPath";

    /// <summary>回调验签 Token 的变量名（只走环境变量）。</summary>
    public const string CallbackTokenEnv = "WORK_CALLBACK_TOKEN";

    /// <summary>回调 EncodingAESKey 的变量名（只走环境变量；长度须 43）。</summary>
    public const string CallbackAesKeyEnv = "WORK_CALLBACK_AES_KEY";

    /// <summary>审批单回捞式模板发现的开关变量名（<c>=1</c> 才启用，默认关）。</summary>
    public const string DiscoveryEnv = "WORK_DISCOVERY";
}
