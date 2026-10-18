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
/// 微信小程序「自定义交易组件 V2 · 交易纠纷单」域接口（3 端点：详情 / 列表 / 上传材料；
/// 官方 <c>minishopopencomponent2/API/complaint</c> 分区）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本域只读 + 补材料</b>：官方仅给「查详情 / 查列表 / 上传材料」三支，<b>没有</b>接受、拒绝、
/// 申诉或关闭纠纷单的写口 ⇒ 商家侧处置须在微信后台完成，SDK 不得臆造状态推进端点。
/// </para>
/// <para>
/// <b>枚举值官方未发布</b>：<c>state</c> / <c>type</c> / <c>judge_result</c> 的注释只写
/// 「见 ComplaintOrderState / ComplaintOrderType」，但本域<b>没有</b>任何枚举表或数据结构页
/// （2026-10-18 逐页取证；<c>complaint/data_structure.html</c> 等候选路径均 404）
/// ⇒ SDK 只建模 <c>int</c>，<b>不凭记忆补枚举常量</b>。
/// </para>
/// <para>
/// <b>官方文档自身矛盾（照录）</b>：<c>complaint/get</c> 回包参数表要求 <c>complaint_order_detail</c>
/// 包装，回包示例却给平铺的 <c>flow_info</c> + <c>order</c>；<c>get_list</c> 参数表写 <c>status</c>、
/// 请求示例写 <c>state</c>。DTO 按可空并集承载，不做本地纠正。
/// </para>
/// <para>
/// <b>敏感面</b>：<c>openid</c>、<c>phone_number</c>、<c>order_id</c> 与材料中的图片 URL
/// 均属个人信息 ⇒ 一律禁入日志、遥测与异常消息。
/// </para>
/// <para>
/// <b>调用限制（诚实留档）</b>：3 页官方正文<b>未给</b>端点级频率数字、权限集编号与云调用方法名。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopAfterSale", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopComplaintService
{
    /// <summary>
    /// 获取纠纷单详情（<c>POST /shop/complaint/get</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/complaint/complaint_get.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>complaint_order_id</c>，官方表标选填），见 <see cref="WxaShopComplaintGetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>纠纷单详情与流转记录，见 <see cref="WxaShopComplaintGetResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>回包形态双口径</b>：参数表给嵌套键 <c>complaint_order_detail</c>，示例给顶层
    /// <c>flow_info</c> + <c>order</c> ⇒ 响应类三支全可空，两种形态都能接。
    /// </para>
    /// <para><b>敏感值</b>：应答含 <c>openid</c> / <c>phone_number</c> / <c>order_id</c>，禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/complaint/get")]
    Task<WxaShopComplaintGetResponse> GetComplaintAsync(
        [Body] WxaShopComplaintGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取纠纷单列表（<c>POST /shop/complaint/get_list</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/complaint/complaint_get_list.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>offset</c> / <c>limit</c> 必填，时间窗口为秒级），见 <see cref="WxaShopComplaintGetListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>顶层 <c>orders[]</c> + <c>total</c>，见 <see cref="WxaShopComplaintGetListResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>翻页</b>：<c>offset</c> 从 0 开始、<c>limit</c> 最大 50，二者均必填；
    /// <c>begin_create_time</c> / <c>end_create_time</c> 为<b>秒</b>级时间戳（与 V2 售后列表的毫秒不同）。
    /// </para>
    /// <para>
    /// <b>官方字段名矛盾（照录）</b>：参数表写 <c>status</c>，请求示例写 <c>state</c> ⇒
    /// 本方法按参数表；若网关按示例只认 <c>state</c>，须由宿主与官方确认后处置，SDK 不双写。
    /// </para>
    /// <para><b>敏感值</b>：响应含 <c>openid</c> / <c>phone_number</c> / <c>order_id</c>，禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/complaint/get_list")]
    Task<WxaShopComplaintGetListResponse> GetComplaintListAsync(
        [Body] WxaShopComplaintGetListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传纠纷材料（<c>POST /shop/complaint/upload_material</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/complaint/upload_material.html"/>。
    /// </summary>
    /// <param name="request">请求体（纠纷单号 + 材料，均必填），见 <see cref="WxaShopComplaintUploadMaterialRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>追加语义</b>：只向指定纠纷单追加说明文本与图片 URL，<b>不</b>推进纠纷状态；
    /// 重复上传不会覆盖旧材料 ⇒ 非幂等、不得自动重试。
    /// </para>
    /// <para>
    /// <b>图片是 URL 直链</b>：<c>media_url_list</c> 填可访问 URL（本域无 media_id 上传步骤）。
    /// </para>
    /// </remarks>
    [Post("/shop/complaint/upload_material")]
    Task<WxaResponse> UploadComplaintMaterialAsync(
        [Body] WxaShopComplaintUploadMaterialRequest request,
        CancellationToken cancellationToken = default);
}
