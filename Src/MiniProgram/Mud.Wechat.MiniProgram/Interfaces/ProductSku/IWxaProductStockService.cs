// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ProductSku;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」域——库存接口（2 端点）。
/// </summary>
/// <remarks>
/// <para><b>令牌形态</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。</para>
/// <para><b>官方页文件名与路由错位（照实留档）</b>：<c>/product/stock/get</c> 的文档页是
/// <c>sku/get_stock.html</c>，<c>/product/stock/update</c> 的文档页是 <c>sku/up_stock.html</c>；
/// 路径以<b>请求地址</b>为准（<c>/product/stock/*</c>），<b>不是</b> <c>/product/sku/*</c>。</para>
/// <para><b>更新语义二选一</b>：<c>type = 1</c> 全量覆盖（<c>stock_num</c> 须 &gt; 0）；
/// <c>type = 2</c> 增量（正数加、负数减）。增量更新是<b>非幂等</b>的，失败重试前须先 <c>stock/get</c> 对账。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductSku", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductStockService
{
    /// <summary>
    /// 获取库存（<c>POST /product/stock/get</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/sku/get_stock.html"/>。
    /// </summary>
    /// <param name="request">查询请求（商品 ID 二选一 + SKU ID 二选一），见 <see cref="WxaProductStockGetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>库存量 <c>data.stock_num</c>，见 <see cref="WxaProductStockGetResponse"/>。</returns>
    /// <remarks>
    /// <para><b>错误码</b>：<c>100004</c> SKU 不存在、<c>100002</c> <c>product_id</c> 不存在、<c>-1</c> 系统异常。</para>
    /// <para>官方参数表未列「频率限制 / 串行」约束；只读，可重试。</para>
    /// </remarks>
    [Post("/product/stock/get")]
    Task<WxaProductStockGetResponse> GetStockAsync(
        [Body] WxaProductStockGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新库存（<c>POST /product/stock/update</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/sku/up_stock.html"/>。
    /// </summary>
    /// <param name="request">更新请求（<c>type</c> + <c>stock_num</c> 必填），见 <see cref="WxaProductStockUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新时间 <c>data.update_time</c>，见 <see cref="WxaProductStockUpdateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>type 语义</b>（见 <see cref="WxaProductStockUpdateTypes"/>）：1 全量更新（<c>stock_num</c> 必须大于 0）；
    /// 2 增量更新（正数增加库存、负数减少）。</para>
    /// <para><b>幂等性</b>：全量更新幂等；增量更新<b>不幂等</b>，重放会重复加减。</para>
    /// <para><b>错误码</b>：<c>100004</c> SKU 不存在、<c>100002</c> <c>product_id</c> 不存在、<c>-1</c> 系统异常。</para>
    /// </remarks>
    [Post("/product/stock/update")]
    Task<WxaProductStockUpdateResponse> UpdateStockAsync(
        [Body] WxaProductStockUpdateRequest request,
        CancellationToken cancellationToken = default);
}
