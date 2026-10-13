// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「交易保障 · 投诉管理」域 SDK（5 端点：查询投诉单详情 + 商家回应投诉 +
/// 商家补充凭证 + 商家提交退款凭证 + 商家申诉，官方「交易保障」分组 → 投诉管理）。
/// </summary>
/// <remarks>
/// <para>
/// <b>路由族归属裁决（本域最重要的一条边界）</b>：5 端点路由为 <c>/wxaapi/minishop/complaintOrderDetail</c>、
/// <c>/wxaapi/minishop/bussiRespondComplaint</c>、<c>/wxaapi/minishop/bussiSupplyProof</c>、
/// <c>/wxaapi/minishop/bussiSupplyRefund</c>、<c>/wxaapi/minishop/busiAppeal</c> —— 前缀 <c>minishop</c>
/// 是历史遗留命名，但官方 2026-10-13 已把这 5 页<b>整族迁入</b>小程序服务端 API 的
/// <c>transaction-guarantee/complaint</c> 章节，逐页「适用范围」明示「支持『小程序』账号类型调用」，
/// 令牌为小程序 <c>access_token</c>（权限集 id <b>76</b> 代调用）⇒ 归小程序线 TransactionGuarantee 域。
/// 小程序线 P3-R1 的 <c>/wxaapi/minishop/*</c> 零声明裁决已据此<b>收窄</b>为
/// 「小商店店铺 / 商品管理面零声明；官方仍在册的投诉处理 5 条在册」，
/// 小店（Channels）线侧不得声明本族路由。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。
/// </para>
/// <para>
/// <b>路由拼写照官方原文，禁止「顺手规整」（两处反直觉点）</b>：
/// ① 三个商家动作端点是 <c>bussi</c>（三个 s 起头的 <c>bussi</c>），而申诉端点是 <c>busi</c>（<c>busiAppeal</c>），
/// 官方两页拼写不一致；② 本域路由为<b>驼峰</b>动词名，而同分区的 basic 域是 <c>snake_case</c>
/// （<c>get_penalty_list</c>）。全部照录，不做归一化。
/// </para>
/// <para>
/// <b>状态机前置（官方错误码原文）</b>：4 个动作端点共有 <c>309</c>「状态不对（可能是已经转到下一个状态了）」，
/// 官方解决方案栏写「开发者自行校验投诉单状态」⇒ 动作前应先调查询详情端点确认
/// <c>complaintOrder.status</c> / <c>item.itemType</c> + <c>blameResult</c>，据此选择回应 / 补充凭证 / 提交退款 / 申诉。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "TransactionGuarantee", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaTradeComplaintService
{
    /// <summary>
    /// 查询投诉单详情（<c>GET /wxaapi/minishop/complaintOrderDetail</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/complaint/api_getorderdetail.html"/>。
    /// </summary>
    /// <param name="complaintOrderId">投诉单 id（官方参数表类型 string，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>投诉单详情（<c>complaintOrder</c> / <c>item</c> / <c>returnBill</c>），见 <see cref="DataModels.TransactionGuarantee.WxaComplaintDetailResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>GET</b>；Query 携带 <c>access_token</c> 与 <c>complaintOrderId</c>（官方在 GET 端点下给出「请求体」表，但按示例以查询串发送，同域裁决见 <see cref="DataModels.TransactionGuarantee.WxaTradePenaltyListResponse"/> 注）。</para>
    /// <para>
    /// <b>本端点是其余 4 个动作端点的定位键来源（官方原文）</b>：<c>returnBill.returnId</c>
    /// 「需要通过查询投诉单详情接口获取」；商家动作前应以此接口为准读取 <c>status</c> 与 <c>expireTime</c>。
    /// </para>
    /// <para>
    /// <b>图片链接有时效</b>：<c>customerMaterial.mediaIdList</c> 官方明示「该图片 url 有过期时间，
    /// 如需查看图片，需每次重新调用该接口获取」⇒ 不得缓存复用。
    /// </para>
    /// <para>官方错误码：<c>-1</c>（system error，系统繁忙稍后再试）。</para>
    /// </remarks>
    [Get("/wxaapi/minishop/complaintOrderDetail")]
    Task<DataModels.TransactionGuarantee.WxaComplaintDetailResponse> GetComplaintOrderDetailAsync(
        [Query("complaintOrderId")] string complaintOrderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商家回应投诉（<c>POST /wxaapi/minishop/bussiRespondComplaint</c>，同意 / 拒绝和解）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/complaint/api_respondcomplaint.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>content</c> 与 <c>mediaIdList</c> 二选一、<c>complaintOrderId</c>、<c>bussiHandle</c>），见 <see cref="DataModels.TransactionGuarantee.WxaComplaintRespondRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>（权限集 id 76 可代调用）。</para>
    /// <para>
    /// <b>回应后进入用户确认段</b>：<c>bussiHandle</c> 取 <c>1</c> 同意和解 / <c>2</c> 拒绝和解
    /// （见 <see cref="DataModels.TransactionGuarantee.WxaComplaintBussiHandle"/>），
    /// 官方投诉单状态随之进入 <c>203</c>/<c>204</c>「已回应，待用户确认」，
    /// 超时未确认会转为 <c>202</c> / <c>108</c> 等超时分支。
    /// </para>
    /// <para>官方错误码：<c>0</c> 成功 / <c>1</c> 单号不存在 / <c>309</c> 状态不对 / <c>10001</c> 参数异常 / <c>40001</c> access_token 无效。</para>
    /// </remarks>
    [Post("/wxaapi/minishop/bussiRespondComplaint")]
    Task<WxaResponse> RespondComplaintAsync(
        [Body] DataModels.TransactionGuarantee.WxaComplaintRespondRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商家补充凭证（<c>POST /wxaapi/minishop/bussiSupplyProof</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/complaint/api_supplyproof.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>content</c> 与 <c>mediaIdList</c> 二选一 + <c>complaintOrderId</c>；与申诉端点共用类型），见 <see cref="DataModels.TransactionGuarantee.WxaComplaintProofRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>（权限集 id 76）。</para>
    /// <para>
    /// <b>与「商家申诉」端点请求体完全一致</b>（三字段同名同类型同必填）⇒ 共用
    /// <see cref="DataModels.TransactionGuarantee.WxaComplaintProofRequest"/>，裁决依据记于该类型 remarks。
    /// </para>
    /// <para>
    /// <b>适用状态</b>：官方进度节点 <c>itemType</c> <c>16</c>「待商家补充凭证」、
    /// <c>14</c>「待用户补充凭证」、<c>18</c>「平台要求双方补充凭证」；
    /// 商家侧对应 <c>106</c>/<c>108</c> 状态。超时会转入 <c>112</c> 商家补充凭证超时。
    /// </para>
    /// <para>官方错误码：<c>0</c> 成功 / <c>1</c> 单号不存在 / <c>309</c> 状态不对 / <c>10001</c> 参数异常 / <c>40001</c> access_token 无效。</para>
    /// </remarks>
    [Post("/wxaapi/minishop/bussiSupplyProof")]
    Task<WxaResponse> SupplyProofAsync(
        [Body] DataModels.TransactionGuarantee.WxaComplaintProofRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商家提交退款凭证（<c>POST /wxaapi/minishop/bussiSupplyRefund</c>，退货场景的处理凭证）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/complaint/api_submitrefund.html"/>。
    /// </summary>
    /// <param name="request">请求体（三通用字段 + 退货状态下必填的 <c>acceptReturn</c> / <c>returnId</c>），见 <see cref="DataModels.TransactionGuarantee.WxaComplaintRefundProofRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>（权限集 id 76）。</para>
    /// <para>
    /// <b>退货分支专用</b>：当平台判责为商责且经过退货（进度 <c>itemType</c> 31/32 且 <c>blameResult == 1</c>，
    /// 单状态 <c>308</c>/<c>309</c>「待用户退货中」）时走本端点，须回传 <c>returnId</c>
    /// （由 <see cref="GetComplaintOrderDetailAsync"/> 的 <c>returnBill</c> 取得）；
    /// 未走退货（<c>blameResult == 0</c>「待上传处理凭证」）时走 <see cref="SupplyProofAsync"/>。
    /// </para>
    /// <para>
    /// <b>官方未公布 <c>acceptReturn</c> 枚举</b>：参数表只给语义「确认是否收货（退货状态下必填）」，
    /// 无枚举表 ⇒ 不建常量类（见 <see cref="DataModels.TransactionGuarantee.WxaComplaintRefundProofRequest"/>）。
    /// </para>
    /// <para>官方错误码：<c>0</c> 成功 / <c>1</c> 单号不存在 / <c>309</c> 状态不对 / <c>10001</c> 参数异常 / <c>40001</c> access_token 无效。</para>
    /// </remarks>
    [Post("/wxaapi/minishop/bussiSupplyRefund")]
    Task<WxaResponse> SupplyRefundAsync(
        [Body] DataModels.TransactionGuarantee.WxaComplaintRefundProofRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商家申诉（<c>POST /wxaapi/minishop/busiAppeal</c>，对平台判责结果提出申诉）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/complaint/api_busiappeal.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>content</c> 与 <c>mediaIdList</c> 二选一 + <c>complaintOrderId</c>；与补充凭证端点共用类型），见 <see cref="DataModels.TransactionGuarantee.WxaComplaintProofRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（<c>0</c> 表示成功）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> ＋ 请求体 JSON；Query 携带 <c>access_token</c>（权限集 id 76）。</para>
    /// <para>
    /// <b>申诉窗口（官方枚举）</b>：<c>complaintOrder.appealState</c> 为 <c>401</c>「待商家申诉」时才可发起；
    /// <c>402</c> 已超过申诉时间、<c>403</c> 申诉中、<c>117</c> 申诉成功、<c>118</c> 申诉失败、<c>0</c> 未进入申诉阶段。
    /// </para>
    /// <para>
    /// <b>本端点独有的官方错误码</b>：<c>2</c>「传入 mediaIdList 或 content 格式有误」、
    /// <c>1002</c>「当前状态不支持该操作」——其余为 <c>0</c> / <c>1</c> / <c>10001</c>。
    /// </para>
    /// <para><b>路由拼写照录</b>：本端点为 <c>busiAppeal</c>（<c>busi</c>），其余三个商家动作为 <c>bussi</c>，官方两族拼写不一致。</para>
    /// </remarks>
    [Post("/wxaapi/minishop/busiAppeal")]
    Task<WxaResponse> AppealAsync(
        [Body] DataModels.TransactionGuarantee.WxaComplaintProofRequest request,
        CancellationToken cancellationToken = default);
}
