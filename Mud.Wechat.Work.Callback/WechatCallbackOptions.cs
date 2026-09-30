// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 企业微信回调接收配置（回调运维面，对应 Mud.Feishu.Webhook 的配置包）。
/// </summary>
/// <remarks>
/// 原主配置中的 <c>PushEncodingAESKey</c> / <c>PushToken</c> 迁入本配置
/// （回调推送密钥属回调运维面，不进主配置，见产品规划 §7.1 / 详细设计 §8.4）。
/// </remarks>
public class WechatCallbackOptions
{
    /// <summary>
    /// 回调推送加解密 Token（URL 参数 msg_signature 验签密钥）。
    /// </summary>
    public string PushToken { get; set; } = string.Empty;

    /// <summary>
    /// 回调消息加解密密钥 EncodingAESKey（43 位字符，AES-256-CBC）。
    /// </summary>
    public string PushEncodingAESKey { get; set; } = string.Empty;

    /// <summary>
    /// 回调事件接收的 CorpId（验签时 receiveid 参与签名；服务商模式为企业 CorpId）。
    /// </summary>
    public string CorpId { get; set; } = string.Empty;

    /// <summary>
    /// 校验回调配置完整性（缺失必填项抛出 <see cref="InvalidOperationException"/>）。
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(PushToken))
        {
            throw new InvalidOperationException("回调配置缺少 PushToken。");
        }

        if (string.IsNullOrWhiteSpace(PushEncodingAESKey) || PushEncodingAESKey.Length != 43)
        {
            throw new InvalidOperationException("回调配置的 PushEncodingAESKey 必须为 43 位字符。");
        }
    }
}
