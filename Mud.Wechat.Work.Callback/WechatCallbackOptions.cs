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
/// 多套件场景每个套件/自建应用各登记一份（见 <c>AddWechatCallbackSuite</c>，统一注册表 P1-3/D11）。
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
    /// 回调报文的<b>接收方 ID</b>（解密明文尾部 <c>receiveid</c>，参与明文完整性校验）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 语义按回调形态区分：<b>企业自建应用回调</b>为企业 <c>CorpId</c>；
    /// <b>第三方应用 / 服务商代开发的套件回调</b>为 <c>SuiteId</c>（即外层 XML ToUserName）。
    /// </para>
    /// <para>
    /// <b>必填</b>（P1-3 统一注册表：本值即多套件注册表键，注册期 fail-fast 校验非空与全表唯一）；
    /// 解密明文的 <c>receiveid</c> 与本值不一致即拒绝，明文未携带 receiveid 时跳过校验并一次性告警
    /// （兼容官方「个人主体第三方为空串」形态）。
    /// </para>
    /// </remarks>
    public string CorpId { get; set; } = string.Empty;

    /// <summary>
    /// 校验回调配置完整性（缺失必填项抛出 <see cref="InvalidOperationException"/>）。
    /// </summary>
    /// <remarks>注册期由回调注册表调用（fail-fast），接收器构造期兜底。</remarks>
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

        if (string.IsNullOrWhiteSpace(CorpId))
        {
            throw new InvalidOperationException(
                "回调配置缺少 CorpId（接收方 ID）：企业自建回调请填写企业 CorpId，套件回调请填写 SuiteId。");
        }
    }
}
