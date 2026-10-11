// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「物流发货」域 SDK（28 端点：地址 5 / 运费模板 4 / 电子面单 16 / 订单发货 3）。
/// </summary>
/// <remarks>
/// <para>
/// <b>路由形态（设计方案 v1 §2.2）</b>：本域 28 端点全部 POST，走三组前缀 ——
/// <c>/channels/ec/merchant/address|freight*</c>（地址 5 + 运费模板 4）、
/// <c>/channels/ec/logistics/ewaybill/*</c>（电子面单 16）、<c>/channels/ec/order/delivery*</c>（订单发货 3）。
/// </para>
/// <para>
/// <b>地址（5）</b>：添加 / 获取 / 列表 / 删除 / 更新地址（<c>merchant/address/*</c>）。
/// </para>
/// <para>
/// <b>运费模板（4）</b>：增加 / 查询详情 / 列表 / 更新（<c>merchant/add|get|updatefreighttemplate</c>）。
/// </para>
/// <para>
/// <b>电子面单（16）</b>：取号 create / 预取号 precreate / 查询详情 get / 取消 cancel / 打印成功通知 print /
/// 批量打印通知 batchprint / 子件追加 addsuborder / 开通快递公司列表 delivery/get / 获取打印报文 print/get /
/// 面单标准模板 template/config / 模板 getbyid / 模板信息 get / 新增模板 create / 更新模板 update / 删除模板 delete /
/// 开通网点账号 account/get。
/// </para>
/// <para>
/// <b>订单发货（3）</b>：订单发货 send / 获取快递公司列表 deliverycompanylist/new/get / 订单补发货 compensation。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Logistics", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsLogisticsService
{
    /// <summary>
    /// 添加地址（<c>merchant/address/add</c>）。
    /// </summary>
    /// <param name="request">添加请求（<c>address_detail</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新增地址 id（<c>address_id</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_addaddress"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/merchant/address/add")]
    Task<ChannelsLogisticsAddAddressResponse> AddAddressAsync(
        [Body] ChannelsLogisticsAddAddressRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取地址详情（<c>merchant/address/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>address_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>地址详情（<c>address_detail</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_getaddress"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/merchant/address/get")]
    Task<ChannelsLogisticsGetAddressResponse> GetAddressAsync(
        [Body] ChannelsLogisticsAddressIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取地址列表（<c>merchant/address/list</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>offset</c>+<c>limit</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>地址 id 列表（<c>address_id_list</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_getaddresslist"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/merchant/address/list")]
    Task<ChannelsLogisticsGetAddressListResponse> GetAddressListAsync(
        [Body] ChannelsLogisticsGetAddressListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除地址（<c>merchant/address/delete</c>）。
    /// </summary>
    /// <param name="request">删除请求（<c>address_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_deleteaddress"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/merchant/address/delete")]
    Task<ChannelsResponse> DeleteAddressAsync(
        [Body] ChannelsLogisticsAddressIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新地址（<c>merchant/address/update</c>）。
    /// </summary>
    /// <param name="request">更新请求（<c>address_detail</c> 必填，原地覆盖写入须带全字段）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_updateaddress"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/merchant/address/update")]
    Task<ChannelsResponse> UpdateAddressAsync(
        [Body] ChannelsLogisticsUpdateAddressRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 增加运费模板（<c>merchant/addfreighttemplate</c>）。
    /// </summary>
    /// <param name="request">添加请求（<c>freight_template</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运费模板 id（<c>template_id</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_addfreighttemplate"/></para>
    /// <para>官方业务限制：<c>valuation_type</c> PIECE 按件数 / WEIGHT 按重量；<c>shipping_method</c> FREE 包邮 /
    /// CONDITION_FREE 条件包邮 / NO_FREE 不包邮；<c>delivery_type</c> 仅 EXPRESS 快递。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/merchant/addfreighttemplate")]
    Task<ChannelsLogisticsAddFreightTemplateResponse> AddFreightTemplateAsync(
        [Body] ChannelsLogisticsAddFreightTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询运费模板（<c>merchant/getfreighttemplatedetail</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>template_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运费模板详细信息（<c>freight_template</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_getfreighttemplatedetail"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/merchant/getfreighttemplatedetail")]
    Task<ChannelsLogisticsGetFreightTemplateDetailResponse> GetFreightTemplateDetailAsync(
        [Body] ChannelsLogisticsGetFreightTemplateDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取运费模板列表（<c>merchant/getfreighttemplatelist</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>offset</c>+<c>limit</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运费模板 id 列表（<c>template_id_list</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_getfreighttemplatelist"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/merchant/getfreighttemplatelist")]
    Task<ChannelsLogisticsGetFreightTemplateListResponse> GetFreightTemplateListAsync(
        [Body] ChannelsLogisticsGetFreightTemplateListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新运费模板（<c>merchant/updatefreighttemplate</c>）。
    /// </summary>
    /// <param name="request">更新请求（<c>freight_template</c> 必填，原地覆盖写入须带全字段）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_updatefreighttemplate"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/merchant/updatefreighttemplate")]
    Task<ChannelsResponse> UpdateFreightTemplateAsync(
        [Body] ChannelsLogisticsUpdateFreightTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 电子面单取号（<c>ewaybill/biz/order/create</c>）。
    /// </summary>
    /// <param name="request">取号请求（<c>delivery_id</c>/<c>ewaybill_acct_id</c>/<c>sender</c>/<c>receiver</c>/<c>ec_order_list</c>/<c>shop_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>电子面单订单 id / 运单号 / 子母单号 / 风险与集运信息（<c>ewaybill_order_id</c>/<c>waybill_id</c>…）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_createorder"/></para>
    /// <para>官方业务限制：<c>ewaybill_order_id</c> 数据内容要求 Uint64 以 String 传递；合并寄件须保证 <c>address_info.hash_code</c> 一致。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/logistics/ewaybill/biz/order/create")]
    Task<ChannelsLogisticsEwaybillCreateOrderResponse> CreateEwaybillOrderAsync(
        [Body] ChannelsLogisticsEwaybillCreateOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 电子面单预取号（<c>ewaybill/biz/order/precreate</c>）。
    /// </summary>
    /// <param name="request">预取号请求（<c>delivery_id</c>/<c>ewaybill_acct_id</c>/<c>sender</c>/<c>receiver</c>/<c>ec_order_list</c>/<c>shop_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>全局唯一电子面单订单 id（<c>ewaybill_order_id</c>，用于取号接口）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_precreateorder"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/logistics/ewaybill/biz/order/precreate")]
    Task<ChannelsLogisticsEwaybillPrecreateOrderResponse> PrecreateEwaybillOrderAsync(
        [Body] ChannelsLogisticsEwaybillCreateOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询面单详情（<c>ewaybill/biz/order/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>ewaybill_order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>电子面单详情（<c>order_info</c>，含轨迹 <c>path_info</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_getorder"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/logistics/ewaybill/biz/order/get")]
    Task<ChannelsLogisticsEwaybillGetOrderResponse> GetEwaybillOrderAsync(
        [Body] ChannelsLogisticsEwaybillGetOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 电子面单取消下单（<c>ewaybill/biz/order/cancel</c>）。
    /// </summary>
    /// <param name="request">取消请求（<c>ewaybill_order_id</c>/<c>delivery_id</c>/<c>waybill_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>取消结果与快递公司错误码（<c>delivery_error_msg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_cancelorder"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/logistics/ewaybill/biz/order/cancel")]
    Task<ChannelsLogisticsEwaybillCancelOrderResponse> CancelEwaybillOrderAsync(
        [Body] ChannelsLogisticsEwaybillCancelOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 打印成功通知（<c>ewaybill/biz/order/print</c>）。
    /// </summary>
    /// <param name="request">打印通知请求（<c>ewaybill_order_id</c>/<c>delivery_id</c>/<c>waybill_id</c>/<c>re_print</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_printorder"/></para>
    /// <para>官方业务限制：<c>re_print</c> 1 补打单成功 / 0 首次打单成功。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/logistics/ewaybill/biz/order/print")]
    Task<ChannelsResponse> PrintEwaybillOrderAsync(
        [Body] ChannelsLogisticsEwaybillPrintRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量打印通知（<c>ewaybill/biz/order/batchprint</c>）。
    /// </summary>
    /// <param name="request">批量打印通知请求（<c>req_list</c> 必填，单次不超过 50 个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功与失败的单号列表（<c>succ_ewaybill_order_id</c>/<c>fail_ewaybill_order_id</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_batchprintorder"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/logistics/ewaybill/biz/order/batchprint")]
    Task<ChannelsLogisticsEwaybillBatchPrintOrderResponse> BatchPrintEwaybillOrderAsync(
        [Body] ChannelsLogisticsEwaybillBatchPrintOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 电子面单子件追加（<c>ewaybill/biz/order/addsuborder</c>）。
    /// </summary>
    /// <param name="request">追加请求（<c>ewaybill_order_id</c>/<c>waybill_id</c>/<c>delivery_id</c>/<c>add_package_quantity</c>/<c>shop_id</c>/<c>ewaybill_acct_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>子母单号列表 / 打印报文（<c>waybill_id_list</c>/<c>print_info</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_addsuborder"/></para>
    /// <para>官方业务限制：<c>add_package_quantity</c> 1 ≤ value ≤ 300；支持子母单的快递公司：顺丰 / 德邦 / 京东 / 顺心捷达 / 韵达快运 / 安能物流。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/logistics/ewaybill/biz/order/addsuborder")]
    Task<ChannelsLogisticsEwaybillAddSubOrderResponse> AddEwaybillSubOrderAsync(
        [Body] ChannelsLogisticsEwaybillAddSubOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询开通的快递公司列表（<c>ewaybill/biz/delivery/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>delivery_id</c>/<c>status</c> 可选过滤）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>店铺 id 与快递公司列表（<c>shop_id</c>/<c>list</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_getdeliverylist"/></para>
    /// <para>官方业务限制：<c>status</c> 1 绑定审核中；2 取消绑定审核中；3 已绑定；4 已解除绑定；5 绑定未通过；6 取消绑定未通过。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/logistics/ewaybill/biz/delivery/get")]
    Task<ChannelsLogisticsEwaybillGetDeliveryListResponse> GetEwaybillDeliveryListAsync(
        [Body] ChannelsLogisticsEwaybillGetDeliveryListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取打印报文（<c>ewaybill/biz/print/get</c>）。
    /// </summary>
    /// <param name="request">获取请求（<c>ewaybill_order_id</c>/<c>template_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>电子面单打印报文（<c>print_info</c>，打单时传给打印机使用）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_get_print_content_supplier"/></para>
    /// <para>官方业务限制：<c>sub_waybill_id</c> 不传则返回整单的打印报文。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/logistics/ewaybill/biz/print/get")]
    Task<ChannelsLogisticsEwaybillGetPrintContentResponse> GetEwaybillPrintContentAsync(
        [Body] ChannelsLogisticsEwaybillGetPrintContentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取面单标准模板（<c>ewaybill/biz/template/config</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>所有快递公司模板信息汇总（<c>config</c>，键为快递公司 delivery_id）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_get_template_config"/></para>
    /// <para>官方业务限制：本接口<b>无请求体</b>（请求体 Request Payload：无）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/logistics/ewaybill/biz/template/config")]
    Task<ChannelsLogisticsEwaybillGetTemplateConfigResponse> GetEwaybillTemplateConfigAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据模板 ID 获取面单模板信息（<c>ewaybill/biz/template/getbyid</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>delivery_id</c>/<c>template_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>模板信息（<c>template_info</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_gettemplatebyid"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/logistics/ewaybill/biz/template/getbyid")]
    Task<ChannelsLogisticsEwaybillGetTemplateByIdResponse> GetEwaybillTemplateByIdAsync(
        [Body] ChannelsLogisticsEwaybillGetTemplateByIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取面单模板信息（<c>ewaybill/biz/template/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>delivery_id</c> 可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>所有快递公司模板信息汇总（<c>total_template</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_gettemplate"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/logistics/ewaybill/biz/template/get")]
    Task<ChannelsLogisticsEwaybillGetTemplateResponse> GetEwaybillTemplateAsync(
        [Body] ChannelsLogisticsEwaybillGetTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增面单模板（<c>ewaybill/biz/template/create</c>）。
    /// </summary>
    /// <param name="request">新增请求（<c>delivery_id</c>/<c>info</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>快递公司编码与模板编码（<c>delivery_id</c>/<c>template_id</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_createtemplate"/></para>
    /// <para>官方业务限制：<c>options</c> 总数必须为 8 个且 <c>option_id</c> 不得重复；<c>template_name</c> 同一快递公司下不可重复。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/logistics/ewaybill/biz/template/create")]
    Task<ChannelsLogisticsEwaybillCreateTemplateResponse> CreateEwaybillTemplateAsync(
        [Body] ChannelsLogisticsEwaybillCreateTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新面单模板（<c>ewaybill/biz/template/update</c>）。
    /// </summary>
    /// <param name="request">更新请求（<c>delivery_id</c>/<c>info</c> 必填，原地覆盖写入须带全字段）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_updatetemplate"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/logistics/ewaybill/biz/template/update")]
    Task<ChannelsResponse> UpdateEwaybillTemplateAsync(
        [Body] ChannelsLogisticsEwaybillUpdateTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除面单模板（<c>ewaybill/biz/template/delete</c>）。
    /// </summary>
    /// <param name="request">删除请求（<c>delivery_id</c>/<c>template_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_deltemplate"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/logistics/ewaybill/biz/template/delete")]
    Task<ChannelsResponse> DeleteEwaybillTemplateAsync(
        [Body] ChannelsLogisticsEwaybillDeleteTemplateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询开通的电子面单网点 / 账号信息（<c>ewaybill/biz/account/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>need_balance</c>/<c>limit</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>总条数与账号列表（<c>total_num</c>/<c>account_list</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/ewaybill/api_ewaybill_getacct"/></para>
    /// <para>官方业务限制：<c>need_balance=true</c> 时 <c>limit</c> 建议小于 20，否则 limit 过大导致接口超时无返回。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/logistics/ewaybill/biz/account/get")]
    Task<ChannelsLogisticsEwaybillGetAcctResponse> GetEwaybillAcctAsync(
        [Body] ChannelsLogisticsEwaybillGetAcctRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 订单发货（<c>order/delivery/send</c>）。
    /// </summary>
    /// <param name="request">发货请求（<c>order_id</c>/<c>delivery_list</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_senddelivery"/></para>
    /// <para>官方业务限制：<c>deliver_type</c> 1 自寄快递发货 / 3 虚拟商品无需物流发货（仅 deliver_method=1 的订单可用）；<c>delivery_id</c> 非主流可填 OTHER。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/delivery/send")]
    Task<ChannelsResponse> SendDeliveryAsync(
        [Body] ChannelsLogisticsSendDeliveryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取快递公司列表（<c>order/deliverycompanylist/new/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>ewaybill_only</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>快递公司列表（<c>company_list</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_getdeliverycompanylistnew"/></para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/deliverycompanylist/new/get")]
    Task<ChannelsLogisticsGetDeliveryCompanyListResponse> GetDeliveryCompanyListAsync(
        [Body] ChannelsLogisticsGetDeliveryCompanyListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 订单补发货（<c>order/delivery/compensation</c>）。
    /// </summary>
    /// <param name="request">补发请求（<c>order_id</c>/<c>delivery_list</c>/<c>reason</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成功 / 失败（<c>errcode</c>/<c>errmsg</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/channels-shop-logistics/api_deliverycompensation"/></para>
    /// <para>官方业务限制：一个订单最多可补发 10 次；<c>reason</c> 1 商品漏发 / 2 商品拆分包裹 / 3 商品坏损 / 4 赠品。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/delivery/compensation")]
    Task<ChannelsResponse> DeliveryCompensationAsync(
        [Body] ChannelsLogisticsDeliveryCompensationRequest request,
        CancellationToken cancellationToken = default);
}
