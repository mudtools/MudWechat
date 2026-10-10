// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Abstractions.Authentication;

/// <summary>
/// 微信小店 / 视频号 <c>access_token</c> 令牌管理器契约（普通通道 / 稳定版通道共用同一契约）。
/// </summary>
/// <remarks>
/// <para>
/// 具体实现（<c>ChannelsStableAccessTokenManager</c> / <c>ChannelsStandardAccessTokenManager</c>）为 internal，
/// 由 <c>AddChannelsApp</c> 按 <see cref="Configuration.ChannelsAppConfig.UseStableToken"/> 在注册期装配；
/// 对外只暴露本契约（消费方经 <c>IChannelsAppContext.GetTokenManager(ChannelsTokenTypes.AccessToken)</c> 或
/// <c>IChannelsAppManager.DefaultAccessTokenManager</c> 获取）。
/// </para>
/// <para>
/// <b>路由键唯一（客户端接入不感知通道）</b>：<c>[Token(TokenType = ChannelsTokenTypes.AccessToken)]</c>
/// 是编译期常量且被生成代码写入 <c>TokenRecoveryContext</c>（恢复链路按它解析管理器）。若把通道编进查找键，
/// 而「走哪条通道」是运行期配置（<see cref="Configuration.ChannelsAppConfig.UseStableToken"/>），注入路径与
/// 恢复路径会分裂到两个管理器 —— 故通道差异只落在<b>注册期选择的实现</b>与<b>存储键前缀</b>两处
/// （对齐公众号线 <c>MpTokenTypes</c> 同款裁定，见 <c>ChannelsTokenTypes</c> remarks）。
/// </para>
/// </remarks>
public interface IChannelsAccessTokenManager : ITokenManager
{
}