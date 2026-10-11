// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Warehouse;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「区域仓库」域 SDK（11 端点：仓库创建 / 列表 / 详情 /
/// 修改 / 覆盖区域批量增删 / 地址优先级查设 / 库存更新查询 + 地址行政编码）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与「本地生活门店」「质检仓」概念严格区分</b>：本域是微信小店<b>发货侧区域仓库</b>
/// （<c>/channels/ec/warehouse/*</c>，小店 <c>access_token</c>，第三方代调用权限集 id 129）；
/// 质检送检仓为「质检管理」域（<c>/channels/ec/qic/*</c>），本地生活门店为 Locallife 域。
/// </para>
/// <para>
/// <b>路由前缀并存</b>：本域 10 端点走 <c>/channels/ec/warehouse/*</c>，1 端点
/// （<c>获取地址行政编码</c>）走 <c>/channels/ec/basics/addresscode/get</c>（官方将该端点
/// 归入区域仓库 API 族，路由为 basics 历史前缀，照抄官方原文不得改写）。
/// </para>
/// <para>
/// <b>地址编码口径</b>：地址行政编码为 9 位数字（如广东省 <c>440000000</c>），来源为
/// <c>获取地址行政编码</c> 端点；覆盖区域按「从右往左、0 为通配」规则匹配（见各方法 remarks）。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Warehouse", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsWarehouseService
{
    /// <summary>
    /// 创建区域仓库（<c>createwarehouse</c>）。
    /// </summary>
    /// <param name="request">创建请求（<c>out_warehouse_id</c>/<c>name</c>/<c>intro</c> 必填，<c>cover_locations</c> 可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_createwarehouse"/></para>
    /// <para>
    /// 官方业务限制：①<c>out_warehouse_id</c> 为外部仓库 ID，命名规则为字母数字 <c>[0-9, a-z, A-Z]</c>
    /// 加特殊字符 <c>[-_.]</c>，最大 128 字符，<b>一个店铺下同一个外部 ID 只能创建一个仓库</b>；
    /// ②<c>cover_locations</c> 覆盖规则（A/B/C/D 均非 0）：仅填 <c>address_id1</c>=覆盖 A 省份，
    /// 填 1、2=覆盖 A 省 B 市，填 1~3=覆盖 A 省 B 市 C 区，填 1~4=覆盖 A 省 B 市 C 区 D 街道；
    /// ③覆盖区域可在创建后经「批量增加覆盖区域」接口添加。
    /// </para>
    /// <para>官方错误码：<c>10020200</c>（out_warehouse_id 命名非法）/ <c>10020203</c>（address_id 非法或关联关系非法）/ <c>10020205</c>（仓库名称或仓库介绍为空）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/warehouse/create")]
    Task<ChannelsResponse> CreateWarehouseAsync(
        [Body] ChannelsCreateWarehouseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询区域仓库列表（<c>list/get</c>，查询结果按创建时间升序排序）。
    /// </summary>
    /// <param name="request">查询请求（<c>page_size</c> 必填且最大不超过 10；<c>next_key</c> 翻页上下文选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>外部仓库 ID 列表与翻页上下文（<c>out_warehouse_ids</c>/<c>next_key</c>，<c>next_key</c> 为空表示当前是最后一页）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_getwarehouselist"/></para>
    /// <para>
    /// 官方业务限制：①<c>page_size</c> 最大不超过 10；②查询结果<b>按创建时间升序排序</b>；
    /// ③翻页传上次返回的 <c>next_key</c>（不传默认取第一页），返回的 <c>next_key</c> 为空即最后一页。
    /// </para>
    /// <para>官方错误码：无特殊错误码（走通用错误码）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/warehouse/list/get")]
    Task<ChannelsGetWarehouseListResponse> GetWarehouseListAsync(
        [Body] ChannelsGetWarehouseListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取区域仓库（<c>get</c>，单个仓库详情含覆盖区域）。
    /// </summary>
    /// <param name="request">查询请求（<c>out_warehouse_id</c> 必填，可从查询区域仓库列表接口获取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仓库详情（<c>out_warehouse_id</c>/<c>name</c>/<c>intro</c>/<c>cover_locations</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_getwarehouse"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；返回的 <c>cover_locations</c> 可作为
    /// 「批量增加/删除覆盖区域」的入参来源。</para>
    /// <para>官方错误码：<c>10020202</c>（out_warehouse_id 不存在）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/warehouse/get")]
    Task<ChannelsGetWarehouseResponse> GetWarehouseAsync(
        [Body] ChannelsGetWarehouseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改区域仓库详情（<c>detail/update</c>，仅仓库名称与介绍）。
    /// </summary>
    /// <param name="request">修改请求（<c>out_warehouse_id</c>/<c>name</c>/<c>intro</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_updatewarehousedetail"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；覆盖区域的修改不走本接口，
    /// 须用「批量增加覆盖区域」与「批量删除覆盖区域」两个接口。</para>
    /// <para>官方错误码：<c>10020202</c>（out_warehouse_id 不存在）/ <c>10020205</c>（仓库名称或仓库介绍为空）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/warehouse/detail/update")]
    Task<ChannelsResponse> UpdateWarehouseDetailAsync(
        [Body] ChannelsUpdateWarehouseDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量增加覆盖区域（<c>coverlocations/add</c>）。
    /// </summary>
    /// <param name="request">增加请求（<c>out_warehouse_id</c>/<c>cover_locations</c> 必填；覆盖区域元素可从获取区域仓库接口获得）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_addcoverlocations"/></para>
    /// <para>
    /// 官方业务限制：①覆盖规则（A/B/C/D 均非 0）：仅填 <c>address_id1</c>=覆盖 A 省份，
    /// 填 1、2=覆盖 A 省 B 市，填 1~3=覆盖 A 省 B 市 C 区，填 1~4=覆盖 A 省 B 市 C 区 D 街道；
    /// ②<c>address_id1</c> 必填，其余地址可留空（填 0 视为通配）。
    /// </para>
    /// <para>官方错误码：<c>10020202</c>（out_warehouse_id 不存在）/ <c>10020203</c>（address_id 非法或关联关系非法）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/warehouse/coverlocations/add")]
    Task<ChannelsResponse> AddCoverLocationsAsync(
        [Body] ChannelsWarehouseCoverLocationsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除覆盖区域（<c>coverlocations/del</c>）。
    /// </summary>
    /// <param name="request">删除请求（<c>out_warehouse_id</c>/<c>cover_locations</c> 必填；与批量增加覆盖区域请求体字段集一致 ⇒ 共用请求 DTO）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_delcoverlocations"/></para>
    /// <para>
    /// 官方业务限制：覆盖规则与「批量增加覆盖区域」一致（仅填 <c>address_id1</c>=覆盖 A 省份，
    /// 逐级细化至 A 省 B 市 C 区 D 街道）；<c>address_id1</c> 必填，其余地址可留空。
    /// </para>
    /// <para>官方错误码：<c>10020202</c>（out_warehouse_id 不存在）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/warehouse/coverlocations/del")]
    Task<ChannelsResponse> DeleteCoverLocationsAsync(
        [Body] ChannelsWarehouseCoverLocationsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取指定地址下区域仓库的优先级（<c>address/prioritysort/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>address_id1</c> 省份编码必填，其余级别选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>按优先级从高到低排序的外部仓库 ID 列表（<c>priority_sort</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_getaddressprioritysort"/></para>
    /// <para>
    /// 官方业务限制：①返回的 <c>priority_sort</c> 按 <c>out_warehouse_id</c> 排列、优先级从高到低；
    /// ②未通过「设置指定地址下区域仓库的优先级」设置时，默认按区域仓库创建时间作为优先级
    /// （越早创建优先级越高）；③下单匹配原则按照地址从右往左进行匹配（详见官方「更新区域仓库存数量」接口）。
    /// </para>
    /// <para>官方错误码：无特殊错误码（走通用错误码）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/warehouse/address/prioritysort/get")]
    Task<ChannelsGetAddressPrioritySortResponse> GetAddressPrioritySortAsync(
        [Body] ChannelsGetAddressPrioritySortRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置指定地址下区域仓库的优先级（<c>address/prioritysort/set</c>）。
    /// </summary>
    /// <param name="request">设置请求（<c>address_id1</c> 必填；<c>priority_sort</c> 为按优先级从高到低排列的外部仓库 ID 列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_setaddressprioritysort"/></para>
    /// <para>
    /// 官方业务限制：①若未设置优先级，默认按区域仓库创建时间作为优先级（越早创建优先级越高）；
    /// ②下单匹配原则按照地址从右往左进行匹配；③<c>priority_sort</c> 列表内 <c>out_warehouse_id</c> 不得重复。
    /// </para>
    /// <para>官方错误码：<c>10020202</c>（out_warehouse_id 不存在）/ <c>10020203</c>（address_id 非法或关联关系非法）/ <c>10020206</c>（重复的 out_warehouse_id）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/warehouse/address/prioritysort/set")]
    Task<ChannelsResponse> SetAddressPrioritySortAsync(
        [Body] ChannelsSetAddressPrioritySortRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新区域仓库存数量（<c>stock/update</c>，增加 / 减少 / 设置三种修改类型）。
    /// </summary>
    /// <param name="request">更新请求（<c>product_id</c>/<c>sku_id</c>/<c>out_warehouse_id</c>/<c>diff_type</c>/<c>num</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_updatewarehousestock"/></para>
    /// <para>
    /// 官方业务限制：①修改类型枚举见 <see cref="ChannelsWarehouseStockDiffTypes"/>（1 增加 / 2 减少 / 3 设置）；
    /// ②官方下单匹配原则：<b>按照地址从右往左进行匹配</b>——先筛选库存 ≥ 用户下单数量的区域仓，
    /// 再按地址从右往左逐级匹配（通配符 0 可匹配任意地址，精准匹配优先于通配），
    /// 最后取符合条件集合中优先级最高的区域仓。
    /// </para>
    /// <para>官方错误码：<c>10020052</c>（商品不存在）/ <c>10020202</c>（out_warehouse_id 不存在）/ <c>10020204</c>（该 sku 设置区域仓数量超过限制）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/warehouse/stock/update")]
    Task<ChannelsResponse> UpdateWarehouseStockAsync(
        [Body] ChannelsUpdateWarehouseStockRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取区域仓库存数量（<c>stock/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c>/<c>sku_id</c>/<c>out_warehouse_id</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>库存数量（<c>num</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_getwarehousestock"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；库存值随「更新区域仓库存数量」接口维护。</para>
    /// <para>官方错误码：<c>10020052</c>（商品不存在）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/warehouse/stock/get")]
    Task<ChannelsGetWarehouseStockResponse> GetWarehouseStockAsync(
        [Body] ChannelsGetWarehouseStockRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取地址行政编码（<c>addresscode/get</c>，最多获取 4 级地址的行政编码）。
    /// </summary>
    /// <param name="request">查询请求（<c>addr_code</c> 选填——不填或填 0 时获取全国的省级行政编码）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>本行政编码地址信息（<c>addrs_msg</c>）与下一级所有地址信息（<c>next_level_addrs</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_getaddresscode"/></para>
    /// <para>
    /// 官方业务限制：①最多获取 <b>4 级地址</b>的行政编码（省 / 市 / 区县 / 街道，
    /// 地址级别枚举见 <see cref="ChannelsAddressLevels"/>）；②<c>addr_code</c> 不填或填 0 时
    /// 返回全国省级行政编码（官方返回示例中根节点「中华人民共和国」<c>level</c>=<c>0</c>，照录）；
    /// ③地址编码可逐级下钻，作为「创建区域仓库」等接口 <c>address_id1~4</c> 的取值来源。
    /// </para>
    /// <para>官方错误码：<c>10021402</c>（无效的地址编码）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>；官方适用范围同时放行「微信小店」与「小店供货商」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/basics/addresscode/get")]
    Task<ChannelsGetAddressCodeResponse> GetAddressCodeAsync(
        [Body] ChannelsGetAddressCodeRequest request,
        CancellationToken cancellationToken = default);
}
