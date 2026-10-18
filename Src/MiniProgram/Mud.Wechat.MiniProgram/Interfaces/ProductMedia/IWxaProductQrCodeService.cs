// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.ProductMedia;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」·管理身份二维码域（3 端点：
/// 生成 / 取图 / 检查扫码状态；路由族 <c>/product/qrcode/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>完整握手三支</b>：<c>gen</c> 取 <c>qrcode_ticket</c> → <c>get</c> 用 ticket 换 base64 图片
/// → <c>check</c> 轮询扫码与确认状态 ⇒ ticket 是唯一贯穿键。
/// </para>
/// <para>
/// <b>图片是 base64 而非二进制</b>：<c>qrcode/get</c> 回 <c>qrcode_buf</c> 字符串，调用方须自行
/// base64 解码（官方原文）。
/// </para>
/// <para>
/// <b>官方枚举表头重复（照录）</b>：<c>check</c> 页两段枚举都标成「枚举值-status」，
/// 第二段实为 <c>scan_user_type</c> ⇒ SDK 按字段语义拆成两个常量类。
/// </para>
/// <para>
/// <b>调用限制（诚实留档）</b>：本族 3 页官方正文未给端点级频率数字、权限集编号与云调用方法名
/// （2026-10-18 逐页取证）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductMedia", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductQrCodeService
{
    /// <summary>
    /// 生成管理身份二维码（<c>POST /product/qrcode/gen</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/qrcode/gen_qrcode.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>type</c>，当前仅 1 管理员），见 <see cref="WxaProductQrCodeGenRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>qrcode_ticket</c>，见 <see cref="WxaProductQrCodeGenResponse"/>。</returns>
    /// <remarks>
    /// <para><b>非幂等</b>：每次调用生成新 ticket，旧 ticket 是否失效官方未说明 ⇒ 缓存复用同一 ticket。</para>
    /// </remarks>
    [Post("/product/qrcode/gen")]
    Task<WxaProductQrCodeGenResponse> GenQrCodeAsync(
        [Body] WxaProductQrCodeGenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取二维码图片（<c>POST /product/qrcode/get</c>，官方「查询」类端点仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/qrcode/get_qrcode.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>ticket</c>），见 <see cref="WxaProductQrCodeGetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>qrcode_buf</c>（<b>base64</b> 编码二进制），见 <see cref="WxaProductQrCodeGetResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>须 base64 解码</b>：官方明示 <c>qrcode_buf</c> 为 base64 编码二进制；
    /// 错误码 60220 ticket 已失效 / 60208 错误的 ticket。
    /// </para>
    /// </remarks>
    [Post("/product/qrcode/get")]
    Task<WxaProductQrCodeGetResponse> GetQrCodeAsync(
        [Body] WxaProductQrCodeGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查扫码状态（<c>POST /product/qrcode/check</c>，官方「查询」类端点仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/qrcode/check_qrcode.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>ticket</c>），见 <see cref="WxaProductQrCodeCheckRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>扫码状态 + 业务错误 + 扫码者身份，见 <see cref="WxaProductQrCodeCheckResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>两层错误面</b>：顶层 <c>errcode</c> 是接口调用结果；<c>self_check_err_code</c> /
    /// <c>self_check_err_msg</c> 是扫码校验业务结果，两者须分别判读。
    /// </para>
    /// <para>
    /// <b>轮询语义</b>：<c>status</c> 0 未扫码 / 1 已确认 / 2 已取消 / 3 已失效 / 4 已扫码 ⇒
    /// 宿主按 1（成功）、2/3（终态失败）停止轮询，0/4 可继续。
    /// </para>
    /// </remarks>
    [Post("/product/qrcode/check")]
    Task<WxaProductQrCodeCheckResponse> CheckQrCodeAsync(
        [Body] WxaProductQrCodeCheckRequest request,
        CancellationToken cancellationToken = default);
}
