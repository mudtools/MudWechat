// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.ProductCoupon;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」·限时抢购域（3 端点：新增任务 / 任务列表 /
/// 更新任务状态；路由族 <c>/product/limiteddiscount/*</c>，官方页面归档在 <c>API/spu/</c> 分区）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方页面位于 spu 分区但路由独立</b>：三页文档 URL 都在 <c>API/spu/</c> 下
/// （<c>add_limiteddiscount.html</c> 等），请求地址却是 <c>/product/limiteddiscount/*</c>
/// ⇒ 路由以正文请求地址为准。
/// </para>
/// <para>
/// <b>金额 / 时间口径</b>：<c>sale_price</c> 单位为分且必须小于原价；任务时间与列表回包时间
/// 均为 Unix <b>秒</b>级时间戳（number），与优惠券域响应侧字符串时间不同源。
/// </para>
/// <para>
/// <b>一商品一任务</b>：同一 <c>product_id</c> 已有进行中的抢购任务时返回 110000。
/// </para>
/// <para>
/// <b>状态机不完整（诚实留档）</b>：官方未发布任务状态完整枚举；<c>get_list</c> 的
/// <c>status</c> 说明为「拉取的抢购任务状态」但无值域表，示例只出现 0；仅 <c>update_status</c>
/// 明示 1 结束 / 9 删除 ⇒ SDK 不臆造其它值。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductCoupon", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductLimitedDiscountService
{
    /// <summary>
    /// 新增限时抢购任务（<c>POST /product/limiteddiscount/add</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/spu/add_limiteddiscount.html"/>。
    /// </summary>
    /// <param name="request">请求体（商品 + 时间窗口 + SKU 抢购价与库存），见 <see cref="WxaProductLimitedDiscountAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>顶层 <c>task_id</c>，见 <see cref="WxaProductLimitedDiscountAddResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>时间窗口硬约束</b>：<c>end_time</c> 必须大于当前时间且大于 <c>start_time</c>，
    /// 违者 110003。
    /// </para>
    /// <para>
    /// <b>价格 / 库存约束</b>：<c>sale_price</c> 必须小于原价；<c>sale_stock</c> 必须小于等于
    /// 现有库存（官方原文）⇒ 失败时须先对账 SKU 当前价与库存。
    /// </para>
    /// <para><b>非幂等</b>：成功即产生新任务，重复提交按 110000 拒绝。</para>
    /// </remarks>
    [Post("/product/limiteddiscount/add")]
    Task<WxaProductLimitedDiscountAddResponse> AddLimitedDiscountAsync(
        [Body] WxaProductLimitedDiscountAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取限时抢购任务列表（<c>POST /product/limiteddiscount/get_list</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/spu/get_limiteddiscount_list.html"/>。
    /// </summary>
    /// <param name="request">请求体（可选状态过滤），见 <see cref="WxaProductLimitedDiscountGetListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>顶层 <c>limited_discount_list[]</c>，见 <see cref="WxaProductLimitedDiscountGetListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>无分页</b>：官方只回任务列表，无 page / total 字段。</para>
    /// <para>
    /// <b>状态值域未发布</b>：请求 <c>status</c> 只建模为 <c>int</c>，宿主须自行从回包观察可用值，
    /// 不得把 <see cref="WxaProductLimitedDiscountStatuses"/> 的 1 / 9 反推为全部状态。
    /// </para>
    /// </remarks>
    [Post("/product/limiteddiscount/get_list")]
    Task<WxaProductLimitedDiscountGetListResponse> GetLimitedDiscountListAsync(
        [Body] WxaProductLimitedDiscountGetListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新限时抢购任务状态（<c>POST /product/limiteddiscount/update_status</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/spu/update_limiteddiscount_status.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>task_id</c> + 1 结束 / 9 删除），见 <see cref="WxaProductLimitedDiscountUpdateStatusRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>（官方本页回包示例无 <c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>只支持终结操作</b>：1 结束 / 9 删除，其它值返回 110002 ⇒ 状态推进不可逆，
    /// 删除后任务不可恢复，不得自动重试。
    /// </para>
    /// </remarks>
    [Post("/product/limiteddiscount/update_status")]
    Task<WxaResponse> UpdateLimitedDiscountStatusAsync(
        [Body] WxaProductLimitedDiscountUpdateStatusRequest request,
        CancellationToken cancellationToken = default);
}
