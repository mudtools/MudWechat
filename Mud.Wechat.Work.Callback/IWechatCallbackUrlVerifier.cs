// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 回调 URL 验证（官方接入流程第一步：GET msg_signature/timestamp/nonce/echostr）。
/// </summary>
/// <remarks>
/// <para>
/// 独立接口而非扩展 <see cref="IWechatCallbackReceiver"/>：netstandard2.0 无默认接口方法，
/// 向既有接口追加成员会破坏外部实现者（决策 D4）。
/// </para>
/// <para>
/// <b>D5 取舍</b>：URL 验证是幂等读（管理端可能反复点击「保存」重试验证），<b>不做指纹去重</b>——
/// 去重会造成「验证被自己上一次消耗」的假失败；时间戳时效窗口已足够抗重放。
/// </para>
/// <para>
/// 多套件场景由 <c>WechatCallbackReceiverGroup</c> 实现（按 receiverId 分发到对应条目接收器）。
/// </para>
/// </remarks>
public interface IWechatCallbackUrlVerifier
{
    /// <summary>
    /// 验证并解密 echostr，返回应原样返回给企业微信的明文消息（1 秒内返回、不能加引号/BOM/换行，由宿主 HTTP 层承担）。
    /// </summary>
    /// <param name="receiverId">接收方 ID：该回调 URL 归属的注册表键（企业自建=CorpId / 套件=SuiteId）。</param>
    /// <param name="urlQuery">验证请求的查询串（含 msg_signature / timestamp / nonce / echostr；容忍前导 <c>?</c>）。</param>
    /// <param name="echostr">加密的验证字符串（echostr 参数值）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>解密后的明文消息（<c>msg</c> 部分），宿主必须原样返回给企业微信。</returns>
    /// <exception cref="WechatCallbackException">验签失败、时间窗越界、解密失败或接收方 ID 未注册时抛出。</exception>
    Task<string> VerifyUrlAsync(string receiverId, string urlQuery, string echostr, CancellationToken cancellationToken = default);
}
