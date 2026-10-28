// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Abstractions;

/// <summary>
/// 微信小店 / 视频号（channels 生态）官方令牌类型常量。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何必须新增独立令牌类型（设计方案 v1 §3.2）</b>：小店 AppID 与公众号 / 小程序 AppID
/// <b>不互通</b>，不复用 <c>MpTokenTypes.AccessToken</c>（MP-X2 的复用前提「同一 AppID 双线可用」
/// 在本线不成立）；复用会污染 <c>MpTokenManagerRegistry</c> 单槽（其按 <c>Wechat.Mp.AccessToken</c>
/// 键控，新令牌类型 <c>Resolve</c> 返回 <c>null</c> ⇒ errcode 令牌自愈<b>静默失效</b>）。
/// </para>
/// <para>
/// <b>键值语义是「官方报文参数名契约」</b>：<see cref="AccessToken"/> → Query 参数 <c>access_token</c>，
/// 映射由官方契约锁定，契约守卫 <c>ChannelsTokenOwnerContractGuards</c>（CH-T2）断言不漂移。
/// 键值采用 <c>"Wechat.Channels."</c> 前缀命名空间，避免与企微 <c>Wechat.AccessToken</c> /
/// 公众号 <c>Wechat.Mp.AccessToken</c> 在共享 <c>ITokenManagerRegistry</c> 中冲突。
/// </para>
/// <para>
/// <b>双通道（普通 / 稳定版）只体现于注册期实现的装配</b>：<c>[Token(TokenManagerKey)]</c> 是编译期常量，
/// 若把通道编进查找键，而「走哪条通道」是运行期配置（<see cref="ChannelsAppConfig.UseStableToken"/>），
/// 则注入路径与恢复路径会分裂到两个管理器。故通道差异只落在<b>注册期选择的实现</b>与<b>存储键前缀</b>
/// 两处，路由键保持唯一（对齐公众号线 <c>MpTokenTypes</c> 同款裁定）。
/// </para>
/// </remarks>
public static class ChannelsTokenTypes
{
    /// <summary>小店全局后台接口调用凭据（Query 注入 <c>access_token</c>；普通 / 稳定两通道共用本路由键）。</summary>
    public const string AccessToken = "Wechat.Channels.AccessToken";
}