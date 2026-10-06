// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions;

/// <summary>
/// 微信公众号 / 服务号令牌类型常量（<c>"Wechat.Mp."</c> 前缀，避免与企微 <c>Wechat.AccessToken</c>
/// 在共享 <c>ITokenManagerRegistry</c> 中冲突）。
/// </summary>
/// <remarks>
/// <para>
/// <b>只有一种令牌类型</b>：<c>access_token</c>（全局后台接口调用凭据）。
/// </para>
/// <para>
/// <b>为何不为「普通通道 / 稳定版通道」各设一个查找键</b>：<c>[Token(TokenManagerKey = …)]</c> 是
/// <b>编译期常量</b>并被生成代码写入 <c>TokenRecoveryContext</c>（恢复链路按它解析管理器）。
/// 若把通道编进查找键，而「走哪条通道」是运行期配置（<see cref="Configuration.MpAppConfig.UseStableToken"/>），
/// 则注入路径与恢复路径会分裂到两个管理器（且配置开关形同虚设）。故通道差异只落在
/// <b>注册期选择的实现</b>与<b>存储键前缀</b>两处，路由键保持唯一。
/// </para>
/// </remarks>
public static class MpTokenTypes
{
    /// <summary>公众号全局后台接口调用凭据（Query 注入 <c>access_token</c>；普通 / 稳定两通道共用本路由键）。</summary>
    public const string AccessToken = "Wechat.Mp.AccessToken";
}
