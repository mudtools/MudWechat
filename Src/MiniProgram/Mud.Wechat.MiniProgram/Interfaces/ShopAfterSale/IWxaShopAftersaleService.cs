// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 V2 · V1 售后单」域接口（4 端点：新增 / 查询 / 更新 / 更新售后期限；
/// 官方 <c>minishopopencomponent2/API/aftersale</c> 分区的前四页）。
/// </summary>
/// <remarks>
/// <para>
/// <b>V1 与 V2 是两套并存的售后体系</b>：本接口是「基础交易工具（原标准版交易组件）」的
/// <c>/shop/aftersale/*</c> 四支，商品与金额以<b>商家侧 id + 分</b>表达、售后单只有商家自拟
/// <c>out_aftersale_id</c>；V2 <see cref="IWxaEcAftersaleService"/> 是 <c>/shop/ecaftersale/*</c> 十一支，
/// 有微信侧 <c>aftersale_id</c>、单商品 <c>product_info</c>、毫秒字符串时间与平台介入状态机
/// ⇒ 两族路由、请求体与状态值域<b>全部不同</b>，守卫 SHPO-A1 锁定不得互抄。
/// </para>
/// <para>
/// <b>无删除端点</b>：官方本族只给状态推进（<c>update</c>）与期限调整（<c>update_deadline</c>），
/// 没有删除 / 关闭专用接口；状态 15~19 为换货分支终态，推进<b>不可逆</b> ⇒ 全族不得放进自动重试管道。
/// </para>
/// <para>
/// <b>商家侧主键自拟</b>：<c>out_order_id</c> 与 <c>out_aftersale_id</c> 由宿主生成且须跨批次唯一；
/// 微信不回服务端售后单号，详情 / 更新 / 期限三支均以 <c>out_aftersale_id</c> + <c>openid</c> 定位。
/// </para>
/// <para>
/// <b>敏感面</b>：<c>openid</c>、<c>refund_reason</c>、以及退货地址（V2）属个人信息
/// ⇒ 一律禁入日志、遥测与异常消息。
/// </para>
/// <para>
/// <b>调用限制（诚实留档）</b>：本域 4 页官方正文<b>未给</b>端点级频率数字、权限集编号与云调用方法名
/// （2026-10-18 逐页取证）⇒ 不得凭第三方 SDK 注释或记忆补数。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopAfterSale", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopAftersaleService
{
    /// <summary>
    /// 新增售后单（<c>POST /shop/aftersale/add</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/aftersale/add.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>out_aftersale_id</c> 为必填商家侧主键），见 <see cref="WxaShopAftersaleAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（不回服务端售后单号）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体，<c>openid</c> / <c>out_order_id</c> / <c>out_aftersale_id</c> /
    /// <c>path</c> / <c>refund</c> / <c>type</c> / <c>create_time</c> / <c>status</c> /
    /// <c>finish_all_aftersale</c> / <c>product_infos</c> 全为必填。
    /// </para>
    /// <para>
    /// <b>覆盖语义</b>：同一 <c>out_order_id</c> + <c>out_aftersale_id</c> 重复提交会改写原售后单
    /// ⇒ 非幂等安全，重试必须携带完全一致的请求体。
    /// </para>
    /// <para>
    /// <b>状态由调用方回填</b>：新建通常传 <c>0</c>（未受理），完整值域见
    /// <see cref="WxaShopAftersaleStatuses"/>；<c>refund</c> 单位为<b>分</b>。
    /// </para>
    /// <para><b>敏感值</b>：<c>openid</c> / <c>refund_reason</c> 禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/aftersale/add")]
    Task<WxaResponse> AddAftersaleAsync(
        [Body] WxaShopAftersaleAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取售后单（<c>POST /shop/aftersale/get</c>，官方「查询」类端点仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/aftersale/get.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>out_order_id</c> + <c>openid</c>），见 <see cref="WxaShopAftersaleGetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>顶层 <c>aftersale_infos[]</c>，见 <see cref="WxaShopAftersaleGetResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>一单多售后</b>：回包是列表而不是单对象（同一订单可按商品拆出多条售后单），<b>无</b> total / 分页。
    /// </para>
    /// <para>
    /// <b>官方表头与字段表不一致（照录）</b>：表头称 <c>out_order_id</c> 与 <c>order_id</c> 二选一，
    /// 但参数表只列 <c>out_order_id</c>；本方法请求类按「表 + 示例」并集保留两支。
    /// </para>
    /// <para><b>敏感值</b>：请求与应答中的 <c>openid</c> 禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/aftersale/get")]
    Task<WxaShopAftersaleGetResponse> GetAftersaleAsync(
        [Body] WxaShopAftersaleGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新售后状态（<c>POST /shop/aftersale/update</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/aftersale/update.html"/>。
    /// </summary>
    /// <param name="request">请求体（三定位键 + 目标状态 + 完结标记），见 <see cref="WxaShopAftersaleUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；只改 <c>status</c> 与 <c>finish_all_aftersale</c>，
    /// <b>不</b>重传商品或金额。状态推进不可逆（15~19 为换货分支终态）⇒ 非幂等、不得自动重试。
    /// </para>
    /// <para>
    /// <c>finish_all_aftersale=1</c> 会让<b>整张订单</b>流转售后完结状态 ⇒ 多商品订单须确认其余商品也已处理。
    /// </para>
    /// <para><b>敏感值</b>：<c>openid</c> 禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/aftersale/update")]
    Task<WxaResponse> UpdateAftersaleAsync(
        [Body] WxaShopAftersaleUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新售后期限（<c>POST /shop/aftersale/update_deadline</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/aftersale/update_deadline.html"/>。
    /// </summary>
    /// <param name="request">请求体（订单定位 + <c>after_sale_deadline</c>），见 <see cref="WxaShopAftersaleUpdateDeadlineRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>窗口硬约束</b>：<c>after_sale_deadline</c> 为 Unix <b>秒</b>级时间戳，官方限定
    /// 「当前时间 + [7d，3 × 365d]」⇒ 越界即参数校验失败。
    /// </para>
    /// <para>
    /// <b>官方标题写「更新售后期限」但参数表无售后单号</b>：定位键只有订单级 <c>order_id</c> /
    /// <c>out_order_id</c> + <c>openid</c> ⇒ 作用面是订单维度的可申请售后窗口，不是单条售后单的截止时间（照录）。
    /// </para>
    /// <para><b>敏感值</b>：<c>openid</c> 禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/aftersale/update_deadline")]
    Task<WxaResponse> UpdateAftersaleDeadlineAsync(
        [Body] WxaShopAftersaleUpdateDeadlineRequest request,
        CancellationToken cancellationToken = default);
}
