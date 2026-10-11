// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Window;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「橱窗与本地生活商品」域 SDK（9 端点：视频号橱窗商品 4 / 本地生活团购优惠 5）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域范围（两组，同域不同前缀）</b>：① 视频号橱窗商品（<c>/channels/ec/window/product/*</c>，4 端点，
/// 视频号助手 API，凭据为视频号助手 access_token）；② 本地生活团购优惠（<c>/channels/ec/product/locallife/*</c>，
/// 5 端点，微信小店商家管理商品面）。两组端点照官方原文各自落位，不合并不重复。
/// </para>
/// <para>
/// <b>与小店「主页商品排序」严格区分（同族不同前缀）</b>：本域的 <c>/channels/ec/window/*</c> 是
/// <b>视频号个人橱窗</b>；微信小店店铺主页展示商品走 <c>/channels/ec/store/window/*</c>
/// （见 <see cref="IChannelsHomePageService"/>），不得合并或混用。
/// </para>
/// <para>
/// <b>官方契约警示（照录）</b>：橱窗上下架接口（add/off）官方注明「由于目前橱窗中自营商品的上下架状态会自动同步为
/// 商品在店铺中的状态，该 api 已无实际作用」——保留端点以对齐官方开放面，调用方请优先走小店商品上下架。
/// </para>
/// <para>
/// MUD005 已知接受风险：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Window", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsWindowService
{
    /// <summary>
    /// 上架商品到橱窗（<c>addwindowproduct</c>；官方注明该 API 已无实际作用——自营商品上下架状态自动同步）。
    /// </summary>
    /// <param name="request">上架请求（<c>product_id</c>/<c>appid</c> 必填；<c>is_hide_for_window</c> 选填，默认 false）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/windowproduct/api_addwindowproduct"/></para>
    /// <para>
    /// 官方业务限制：①<b>不支持带货中心来源的商品</b>，其余商品的橱窗商品 ID 与商品来源处的平台内部商品 ID 相同；
    /// ②官方错误码 <c>10022001</c>（验证视频号身份失败）/ <c>10022002</c>（橱窗被禁止使用）/
    /// <c>10022004</c>（商品异常禁止添加）/ <c>10022005</c>（橱窗商品数超过 1 万）/ <c>10022007</c>（不支持操作带货中心来源商品）/
    /// <c>10022008</c>（请求的 appid 不属于该视频号的绑定店铺）；③仅「视频号助手」账号类型可调用。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：参数表 <c>product_id</c> 标注 number、请求示例为带引号字符串——SDK 按参数表建模；
    /// 而同域 get/off/list 三端点的参数表均为 string(uint64)。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>133</c>、<c>177</c>；支持云调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/window/product/add")]
    Task<ChannelsResponse> AddWindowProductAsync(
        [Body] ChannelsAddWindowProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取橱窗商品详情（<c>getwindowproduct</c>，可查自营与带货商品）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c>/<c>appid</c> 必填——<c>need_cps_product</c>=true 时 appid 可留空；<c>need_cps_product</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>自营商品详情（<c>product</c>）与带货商品列表（<c>cps_products</c>，带货计划类型 <c>cps_item_type</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/windowproduct/api_getwindowproduct"/></para>
    /// <para>
    /// 官方业务限制：①自营商品橱窗 ID 即小店商品 ID，带货商品橱窗 ID 为货源商品在小店的商品 ID
    /// （同一货源商品可能对应多个带货商品，每个带货商品 ID 唯一）；②填 <c>need_cps_product</c>=true 后
    /// <c>product_id</c> 支持两种 ID 查询——输入小店商品 ID 返回其全部带货计划，输入带货商品 ID 只返回该带货商品的计划；
    /// ③官方注明 <c>leaf_category_id</c>（叶子类目 ID）将替代 <c>third_category_id</c>，后者最终会下掉，需尽快迁移。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>133</c>、<c>177</c>；支持云调用；仅「视频号助手」账号类型可调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/window/product/get")]
    Task<ChannelsGetWindowProductResponse> GetWindowProductAsync(
        [Body] ChannelsGetWindowProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已添加到橱窗的商品列表（<c>getwindowproductlist</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>page_size</c> 必填且不超过 200；其余选填，<c>appid</c> 与 <c>query_cps_product</c> 互斥——都传时以 appid 优先，仅返回来源指定店铺的自营商品）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>橱窗商品列表（<c>products</c>）与翻页信息（<c>last_buffer</c> 顺序翻页上下文 / <c>total_num</c> 商品总数）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/windowproduct/api_getwindowproductlist"/></para>
    /// <para>
    /// 官方业务限制：①<b>page_size × page_index ≤ 10000</b>，命中限制时改用 <c>last_buffer</c> 顺序翻页
    /// （传入 last_buffer 后 page_index 不生效）；②专属错误码 <c>10022006</c>（请求参数错误，请检查是否符合限制）、
    /// <c>48001</c>（接口权限未开启）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>133</c>、<c>177</c>；支持云调用；仅「视频号助手」账号类型可调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/window/product/list/get")]
    Task<ChannelsGetWindowProductListResponse> GetWindowProductListAsync(
        [Body] ChannelsGetWindowProductListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下架橱窗商品（<c>offwindowproduct</c>；官方注明该 API 已无实际作用——自营商品上下架状态自动同步）。
    /// </summary>
    /// <param name="request">下架请求（<c>product_id</c>/<c>appid</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/windowproduct/api_offwindowproduct"/></para>
    /// <para>
    /// 官方业务限制：①<b>不支持带货中心来源的商品</b>；②专属错误码 <c>10022003</c>（橱窗商品未找到）/
    /// <c>10022007</c>（不支持操作带货中心来源商品）/ <c>10022008</c>（appid 不属于绑定店铺）；③仅「视频号助手」账号类型可调用。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>133</c>、<c>177</c>；支持云调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/window/product/off")]
    Task<ChannelsResponse> OffWindowProductAsync(
        [Body] ChannelsOffWindowProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加团购优惠（<c>addlocalproduct</c>，创建本地团购优惠；<c>product_type</c>/<c>code_source_type</c> 创建后不可改）。
    /// </summary>
    /// <param name="request">创建请求（<c>product_type</c>/<c>product_name</c>/<c>category_id</c>/<c>brand_id</c>/<c>head_imgs</c>/<c>code_source_type</c>/<c>attr_kv_map</c>/<c>skus</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>平台生成的商品 ID（<c>data.product_id</c>）与 SKU ID 列表（<c>data.sku_ids</c>，与请求 skus 顺序一致）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/locallife/shop/api_addlocalproduct"/></para>
    /// <para>
    /// 官方业务限制：①<b>product_type 与 code_source_type 创建后不支持修改</b>；②本接口不支持直接设置库存，
    /// 创建完成后须调用库存管理接口；③请求体 <c>verify_at_store</c>=0 时必须填 <c>verify_page</c>，
    /// 核销体验不达标将被断播处罚；④商品名称 3–60 字符，不得仅为数字或英文，不得含非法字符；
    /// ⑤请求体 <c>attr_kv_map</c> 为字符串键值 map（值为 JSON 字符串化属性，键集参照官方
    /// 「代金券属性KV / 兑换券属性KV」小节）；⑥请求体 <c>head_imgs</c> 须用「上传图片」接口获取；
    /// ⑦专属错误码 <c>10020021</c>（标题过长）/ <c>10020135</c>（use_date 跨度超限：1–180 天，开始不晚于当前+30 天）/
    /// <c>10020149</c>（套餐分组内商品数超出上限 20）/ <c>10020176</c>（售价不能大于原价）等。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>；不支持云调用；仅「微信小店」账号类型可调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/locallife/add")]
    Task<ChannelsAddLocallifeProductResponse> AddLocallifeProductAsync(
        [Body] ChannelsAddLocallifeProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询团购优惠列表（<c>getlocalproductlist</c>，不填 status 默认拉全部商品，不包含回收站）。
    /// </summary>
    /// <param name="request">查询请求（全字段选填：<c>status</c> 商品状态 / <c>page_size</c> 默认 10 不超过 30 / <c>next_key</c> 翻页上下文）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>团购优惠商品 ID 列表（<c>product_ids</c>）与翻页信息（<c>next_key</c>/<c>total_num</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/locallife/shop/api_getlocalproductlist"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；专属错误码 <c>10020150</c>（店铺没有权限调用该接口）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>；不支持云调用；仅「微信小店」账号类型可调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/locallife/list/get")]
    Task<ChannelsGetLocallifeProductListResponse> GetLocallifeProductListAsync(
        [Body] ChannelsGetLocallifeProductListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询团购优惠详情（<c>getlocalproduct</c>，可分别获取线上数据与草稿数据）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c> 必填；<c>data_type</c> 选填，默认取 1：1=线上数据 2=草稿数据 3=同时获取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品线上数据（<c>online_data</c>，data_type=2 时不返回）与草稿数据（<c>draft_data</c>，data_type=1 时不返回；data_type=3 且从未上架过时不返回线上数据）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/locallife/shop/api_getlocalproduct"/></para>
    /// <para>
    /// 官方业务限制：①上架过的商品才有线上数据；②请求体 <c>product_id</c> 为平台生成的 id；
    /// ③专属错误码 <c>10020051</c>（参数错误）/ <c>10020065</c>（商品未上架或不存在）。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：响应参数表把 <c>product_id</c>/<c>category_id</c>/<c>brand_id</c> 标注为 number、
    /// 返回示例为字符串（如 "15000000134007"）——SDK 按返回示例与同域 add/update 请求参数表 string(uint64) 建模为字符串。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>；不支持云调用；仅「微信小店」账号类型可调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/locallife/get")]
    Task<ChannelsGetLocallifeProductResponse> GetLocallifeProductAsync(
        [Body] ChannelsGetLocallifeProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 免审更新团购优惠（<c>updatelocalauditfree</c>，针对已上线的团购优惠免审核更新售卖价格）。
    /// </summary>
    /// <param name="request">更新请求（<c>product_id</c> 必填；<c>skus</c> 必填且更新时 sku_id 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/locallife/shop/api_updatelocalauditfree"/></para>
    /// <para>
    /// 官方业务限制：①仅针对<b>已上线</b>的本地团购优惠；②本接口不支持直接设置库存数量，须调用修改库存接口；
    /// ③更新成功后将通过「团购优惠更新回调」通知商家；④专属错误码 <c>10020013</c>（sku 数量不合理）/
    /// <c>10020034</c>（sku_id 填写有误）/ <c>10020049</c>（商品正在审核中，无法编辑）/ <c>10020176</c>（售价不能大于原价）等。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>；不支持云调用；仅「微信小店」账号类型可调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/locallife/auditfree")]
    Task<ChannelsResponse> AuditFreeUpdateLocallifeProductAsync(
        [Body] ChannelsAuditFreeUpdateLocallifeProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新团购优惠（<c>updatelocalproduct</c>，全量更新；更新成功后通过「团购优惠更新回调」通知商家）。
    /// </summary>
    /// <param name="request">更新请求（<c>product_id</c>/<c>product_type</c>/<c>product_name</c>/<c>category_id</c>/<c>brand_id</c>/<c>head_imgs</c>/<c>code_source_type</c>/<c>attr_kv_map</c>/<c>skus</c> 必填；更新时 sku_id 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/locallife/shop/api_updatelocalproduct"/></para>
    /// <para>
    /// 官方业务限制：①<b>product_type 与 code_source_type 创建后不支持修改</b>；②本接口不支持直接设置库存数量，
    /// 须调用修改库存接口；③请求体 <c>verify_at_store</c>=0 时必须填 <c>verify_page</c>；
    /// ④专属错误码 <c>10020049</c>（商品正在审核中，无法编辑或删除，请先调用撤回商品审核接口）/
    /// <c>10020148</c>（套餐分组数超限：兑换券上限 10 个、次卡限 1 个）/ <c>10020176</c>（售价不能大于原价）等。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>；不支持云调用；仅「微信小店」账号类型可调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/product/locallife/update")]
    Task<ChannelsResponse> UpdateLocallifeProductAsync(
        [Body] ChannelsUpdateLocallifeProductRequest request,
        CancellationToken cancellationToken = default);
}
