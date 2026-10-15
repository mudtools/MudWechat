// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Ecommerce;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「平台收付通 · 二级商户资金账单」域 SDK（<b>1 端点</b>：申请二级商户资金流水账单的下载地址）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-15 逐字段核验）：
/// <see href="https://pay.weixin.qq.com/doc/v3/partner/4012760697"/>，「支持商户」【平台商户】。</para>
/// <para><b>与直连资金账单是两套应答形态，不是「同接口加了个 sub_mchid」</b>：直连
/// <see cref="IWechatPayBillService.GetFundFlowBillAsync"/> 回<b>单文件</b>三字段
/// （<c>hash_type</c>/<c>hash_value</c>/<c>download_url</c>），本支回<b>文件分片列表</b>
/// （<c>download_bill_count</c> + <c>download_bill_list[]</c>，见 <see cref="EcommerceFundFlowBillResponse"/>）
/// ⇒ 必须按 <see cref="EcommerceDownloadBillItem.BillSequence"/>（<b>从 1 起</b>）逐片下载、逐片校验解密后合并。
/// 把直连「下载一个文件即账单」的处理逻辑照搬过来会<b>静默丢数据</b>。</para>
/// <para><b>请求侧也有两处与直连相反</b>：① <c>account_type</c> 官方明文「本接口只支持填 <c>ALL</c>」
/// （直连是按账户类型分列下载），且本支<b>必填</b>；② <c>algorithm</c> 在本支是<b>必填</b>
/// （直连无此要求），取值见 <see cref="EcommerceBillAlgorithms"/>（<c>AEAD_AES_256_GCM</c> / <c>SM4_GCM</c>）。
/// <c>hash_value</c> <b>不是本支的请求参数</b>（摘要值出现在每个分片的应答里）。</para>
/// <para><b>权限可见范围</b>：调用前须先联系微信支付运营申请权限入口，通过后在【服务商平台 → 产品中心】
/// 自助开通「下载二级商户资金流水账单」产品权限 ⇒ 未开通时本支必然失败，属配置缺失而非参数错误。</para>
/// <para><b>时效窗口很窄</b>：① 只支持<b>三个月内</b>的账单，且官方注明二级商户资金流水账单<b>仅能通过本接口下载</b>
/// ⇒ 过期即永久丢失，须自行留存；② 当日账单<b>次日上午 9 点</b>才开始生成，官方建议 10 点后获取；
/// ③ 每个 <c>download_url</c> <b>5 分钟内有效</b> ⇒ 拿到地址必须立刻下载。</para>
/// <para><b>账单金额单位是「元」</b>（官方原文），与本线 API 金额的「分」口径<b>相反</b> ⇒
/// 不要把账单数值与接口数值直接相加对账。</para>
/// <para><b><c>tar_type</c> 选填</b>：不填即返回<b>未压缩</b>数据流，SDK 不代为补默认值（补 <c>GZIP</c>
/// 会让宿主按 gzip 解压而失败）。<c>encrypt_key</c> 是<b>用商户证书公钥加密后 Base64</b> 的对称密钥，
/// 须以自身 API 客户端私钥解出，与 <c>nonce</c> 配套解密账单。</para>
/// <para><b>本支只给下载地址</b>：真实的文件下载走 <c>GET {download_url}</c>（动态路由 + 非 JSON 二进制），
/// 由 <see cref="Mud.Wechat.Pay.Download.IWechatPayBillDownloadService"/> 承载，与直连账单共用同一下载通道
/// ⇒ 本域不重复声明下载端点。</para>
/// <para><b>客户端与凭据</b>：走支付专用命名客户端 <c>wechat-pay</c>，签名主体是当前作用域的平台商户号；
/// 本域<b>零 <c>[Token]</c></b>、<b>无密文入参</b>（Header 参数表只有 <c>Authorization</c> 与 <c>Accept</c>）。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "Ecommerce", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayEcommerceBillService
{
    /// <summary>
    /// 「申请二级商户资金账单」：取该电商平台二级商户资金流水账单的<b>分片下载地址列表</b>。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012760697"/>。
    /// </summary>
    /// <param name="billDate">账单日期（官方 query <c>bill_date</c>，<b>必填</b> string(10)）：<c>yyyy-MM-DD</c>，仅支持三个月内。</param>
    /// <param name="accountType">资金账户类型（官方 query <c>account_type</c>，<b>必填</b>）：本支<b>只接受</b> <c>ALL</c>。</param>
    /// <param name="algorithm">账单文件加密算法（官方 query <c>algorithm</c>，<b>必填</b>）：见 <see cref="EcommerceBillAlgorithms"/>。</param>
    /// <param name="tarType">压缩类型（官方 query <c>tar_type</c>，<b>选填</b>）：不填即返回未压缩数据流，见 <see cref="EcommerceBillTarTypes"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>分片计数与分片列表，见 <see cref="EcommerceFundFlowBillResponse"/>；须按 <c>bill_sequence</c> 合并。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/ecommerce/bill/fundflowbill?bill_date=…&amp;account_type=ALL&amp;algorithm=…&amp;tar_type=…</c>。</para>
    /// <para><b>页内无端点级频率数字</b>（与直连资金账单一致）。</para>
    /// <para><b>业务错误码</b>：400 <c>PARAM_ERROR</c> / 400 <c>INVALID_REQUEST</c> / 401 <c>SIGN_ERROR</c> /
    /// 403 <c>NO_AUTH</c>（未开通产品权限）/ 500 <c>SYSTEM_ERROR</c> / 429 <c>FREQUENCY_LIMITED</c>，
    /// 原样透传至 <see cref="Mud.Wechat.Pay.DataModels.Common.WechatPayResponse.Code"/>。</para>
    /// </remarks>
    [Get("/v3/ecommerce/bill/fundflowbill")]
    Task<EcommerceFundFlowBillResponse> ApplyFundFlowBillAsync(
        [Query("bill_date")] string billDate,
        [Query("account_type")] string accountType,
        [Query("algorithm")] string algorithm,
        [Query("tar_type")] string? tarType = null,
        CancellationToken cancellationToken = default);
}
