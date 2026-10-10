// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics;

namespace Mud.Wechat.Abstractions.Observability;

/// <summary>
/// 微信 SDK 分布式追踪源（全产品线单根源）。
/// </summary>
/// <remarks>
/// <para>
/// 统一所有微信产品线（企业微信 / 公众号 / 小程序 / 开放平台 / 微信支付）的 <see cref="ActivitySource"/>，
/// 便于一次性注册到 OTel SDK。产品线由 <see cref="Tags.Product"/> 标签 + ActivityName 产品线段区分。
/// </para>
/// <para>
/// 业务代码通过 <see cref="Instance"/>.<see cref="ActivitySource.StartActivity(string, ActivityKind)"/>
/// 创建 Activity；无 OTel 消费方时返回 <c>null</c>，零开销降级（<see cref="System.Diagnostics"/> 内建语义）。
/// </para>
/// </remarks>
public static class WechatActivitySource
{
    /// <summary>
    /// ActivitySource 名称，遵循 OTel 命名约定。
    /// </summary>
    public const string Name = "Mud.Wechat";

    /// <summary>
    /// ActivitySource 版本（与 <c>Directory.Build.props</c> 的 <c>&lt;Version&gt;</c> 同步）。
    /// </summary>
    public const string Version = "1.0.3";

    /// <summary>
    /// 静态 ActivitySource 实例。
    /// </summary>
    public static readonly ActivitySource Instance = new(Name, Version);

    /// <summary>
    /// OTel 语义约定与微信 SDK 自定义标签的常量集合（跨产品线共用）。
    /// </summary>
    public static class Tags
    {
        /// <summary>产品线一级维度（取值见 <see cref="Products"/>）</summary>
        public const string Product = "wechat.product";

        /// <summary>应用/账户标识（非秘密，多实例区分维度）</summary>
        public const string AppKey = "wechat.app_key";

        /// <summary>操作结果（success / failure / timeout ...）</summary>
        public const string Outcome = "outcome";

        /// <summary>错误类型名</summary>
        public const string ErrorType = "error.type";

        /// <summary>关联 ID</summary>
        public const string CorrelationId = "wechat.correlation_id";
    }

    /// <summary>
    /// 产品线受控枚举取值（<see cref="Tags.Product"/> 的合法值集合）。
    /// </summary>
    public static class Products
    {
        /// <summary>企业微信</summary>
        public const string Work = "work";

        /// <summary>公众号 / 服务号</summary>
        public const string OfficialAccount = "officialaccount";

        /// <summary>小程序</summary>
        public const string MiniProgram = "miniprogram";

        /// <summary>开放平台</summary>
        public const string OpenPlatform = "openplatform";

        /// <summary>微信支付</summary>
        public const string Pay = "pay";
    }
}
