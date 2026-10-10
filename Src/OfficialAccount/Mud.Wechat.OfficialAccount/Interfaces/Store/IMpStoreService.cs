// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「微信门店 → 门店小程序」域 SDK（12 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 <see href="https://developers.weixin.qq.com/doc/service/api/"/>
/// → 微信门店 → 门店小程序。深链均为**逐页核验过的真实页面**（2026-10-07）。
/// </para>
/// <para>
/// <b>立项前置检查单结论（方案 §4.4 四项，逐页核验产出，勿弱化）</b>：
/// </para>
/// <list type="number">
/// <item><b>行业资质门槛（极窄，关键约束）</b>：<c>apply_merchant</c> 的错误码 <c>43104</c> 官方原文为
/// 「this appid does not have permission__无调用权限，<b>仅开放给电商类目（一级类目：电商平台、商家自营、
/// 跨境电商）</b>」⇒ 非电商类目主体在整个门店小程序链路上不可用（SDK <b>不做本地类目闸</b>，
/// 由官方错误码表达）。另官方给出主体级配额上限：管理员手机 / 微信号 / 身份证 / 主体各 5 次登记
/// （<c>85025</c>~<c>85028</c>）、头像或简介修改每月上限（<c>85049</c>）。</item>
/// <item><b>票据体系</b>：<b>无新票据体系</b>——全部端点消费应用级 <see cref="MpTokenTypes.AccessToken"/>
/// （Query 注入）。<c>add_store</c> / <c>update_store</c> 的 <c>card_id</c> 仅<b>透传</b>卡券 id
/// （官方原文「仅支持会员卡、买单、刷卡支付券，不支持自定义 code」），本域不建模卡券域、
/// 不消费卡券 <c>api_ticket</c>。</item>
/// <item><b>支付耦合</b>：<b>不耦合</b>——「买单 / 刷卡支付券」仅为卡券类型名，本域不调用任何支付接口。</item>
/// <item><b>第三方平台支持面</b>：12 页全部支持代商家调用（<c>authorizer_access_token</c>），
/// 权限集 id <b>8-10、13</b>（8 页）/ <b>8-10、13、37</b>（<c>get_merchant_category</c>、<c>get_district</c>、
/// <c>search_map_poi</c>、<c>create_map_poi</c> 4 页）——按 M0-R3 裁决只在 XML 记录，不扩实现面（与 F5 联动）。</item>
/// </list>
/// <para>
/// <b>账号适用范围（逐页核验，页间不一致，照录）</b>：
/// <c>get_merchant_category</c> / <c>get_district</c> / <c>search_map_poi</c> / <c>create_map_poi</c>
/// 为「小程序 ✔ / 公众号 ✔ / 服务号 ✔」（全开放）；其余 8 页官方适用范围表<b>仅列「公众号 ✔ 服务号 ✔」</b>
/// （小程序列缺失）。<b>全部 12 页均无「仅认证」标注</b>（与模板消息 / OCR 域的「仅认证」口径不同）。
/// SDK 不做账号类型本地闸。
/// </para>
/// <para>
/// <b>频率限制</b>：12 页官方<b>均无频率章节、无频次数值</b>（SDK 不编造数值）。
/// </para>
/// <para>
/// <b>官方文档冲突的处置三段式（本域逐处适用，方案 §4.0 口径来源）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>① 字段存在性冲突</b>（字段表未列而示例有）⇒ <b>取并集</b>（超集建模，不漏数据）——
/// 如 <c>get_store_info</c> 的整个 <c>business.base_info</c> 树（字段表竟只列 <c>errcode</c>/<c>errmsg</c>）。</item>
/// <item><b>② 标量类型冲突</b>（字段表 string ↔ 示例 number 等）⇒ <b>取官方返回示例</b>——
/// 示例是唯一体现真实报文形态的证据，且<b>渲染不会改变标量的 JSON 类型</b>；按字段表建模会被示例直接证伪。
/// 本域命中 3 处：<c>district.location.lat/lng</c>、<c>get_store_list</c> 的
/// <c>longitude</c>/<c>latitude</c>、<c>get_merchant_audit_info</c> 的 <c>reason</c>
/// （表标 number，示例为字符串 <c>""</c>）。</item>
/// <item><b>③ 容器形态冲突</b>⇒ <b>取官方字段表</b>——文档站常把嵌套对象渲染为<b>转义字符串</b>
/// （示例侧的伪影），schema 声明更可信；若为「表把示例的嵌套<b>扁平化</b>」则反转为<b>取示例</b>
/// （表的抽象失真）。本域命中：<c>get_store_list</c> 表把 <c>base_info</c> 子字段列为平级 ⇒ 取示例（嵌套）；
/// <c>add_store.qualification_list</c> 表标 array、示例为单字符串 ⇒ 取表（<c>List&lt;string&gt;</c>）。</item>
/// </list>
/// <para>
/// <b>令牌路由</b>：全部端点消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入
/// <c>access_token</c>（官方契约；MUD005 已知接受风险）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Store", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpStoreService
{
    /// <summary>
    /// 拉取门店小程序类目。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_getwxastorecatelist.html"/>
    /// （官方接口英文名 <c>getwxastorecatelist</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>类目树（<c>data.all_category_info.categories[]</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>，<b>无请求体</b>，Query 仅携带 <c>access_token</c>。
    /// 本端点产出的类目 id 是 <see cref="ApplyMerchantAsync"/> 的 <c>first_catid</c>/<c>second_catid</c> 来源。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>40001</c>（本页错误码表仅此两行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/wxa/get_merchant_category")]
    Task<MpMerchantCategoryResponse> GetMerchantCategoriesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建门店小程序（主体信息）。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_applywxastore.html"/>
    /// （官方接口英文名 <c>applywxastore</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">创建请求（见 <see cref="MpApplyMerchantRequest"/>；类目 id 取自 <see cref="GetMerchantCategoriesAsync"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpApplyMerchantRequest"/>；官方原文「需公众号管理员确认后
    /// 进入审核流程」（结果经 <see cref="GetMerchantAuditInfoAsync"/> 查询）。
    /// </para>
    /// <para>
    /// <b>开放面（官方错误码 <c>43104</c> 原文）</b>：「仅开放给<b>电商类目</b>（一级类目：电商平台、商家自营、
    /// 跨境电商）」，其余类目主体调用即被拒（SDK 不本地拦截）。
    /// </para>
    /// <para>
    /// 官方错误码：<c>40001</c> / <c>43104</c>（无调用权限，仅电商类目）/ <c>85024</c>（需要补充资料）/
    /// <c>85025</c>~<c>85028</c>（手机 / 微信号 / 身份证 / 主体登记数超上限）/ <c>85029</c>~<c>85036</c>
    /// （昵称一族：已被占用 / 长度非法 / 禁用 / 被投诉 / 违法 / 保护期 / 主体不一致、介绍违法）/
    /// <c>85049</c>（头像或简介月修改超限）/ <c>85050</c>（审核中勿重复提交）/ <c>85053</c>（需先成功创建）/ <c>85056</c>（mediaid 无效）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/wxa/apply_merchant")]
    Task<MpResponse> ApplyMerchantAsync(
        [Body] MpApplyMerchantRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取门店小程序审核结果。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_getwxastoreauditinfo.html"/>
    /// （官方接口英文名 <c>getwxastoreauditinfo</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">查询请求（<c>audit_id</c> 必填；取自 <see cref="ApplyMerchantAsync"/> 与门店端点的审核单 id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审核结果（<c>data.audit_id</c>/<c>data.status</c>/<c>data.reason</c>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>方法核验裁决（官方三处矛盾，勿弱化）</b>：官方「调用方式」章标 <b>GET</b>，但「请求参数」章把
    /// <c>audit_id</c> 定义为 <b>REQUEST PAYLOAD（请求体）</b>且标必填，请求示例却为 <c>{}</c>。
    /// SDK 裁决 <b>POST + 请求体</b>：① 参数位置的权威表述是「请求参数」章；② HTTP GET 不应携带请求体，
    /// 若坚持 GET 则 <c>audit_id</c> 无处安放；③ 与同域 merchant / store 其余端点形态一致。官方 GET 标注照录。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/wxa/get_merchant_audit_info")]
    Task<MpMerchantAuditInfoResponse> GetMerchantAuditInfoAsync(
        [Body] MpMerchantAuditInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改门店小程序信息。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_modifywxastore.html"/>
    /// （官方接口英文名 <c>modifywxastore</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">修改请求（<c>headimg_mediaid</c>/<c>intro</c>；不改可传空值）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体见 <see cref="MpModifyMerchantRequest"/>。</para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/wxa/modify_merchant")]
    Task<MpResponse> ModifyMerchantAsync(
        [Body] MpModifyMerchantRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取省市区信息。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_getdistrictlist.html"/>
    /// （官方接口英文名 <c>getdistrictlist</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>省市区二维数组（<c>result</c>：外层省 / 市 / 区三级）+ 数据版本（<c>data_version</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>，<b>无请求体</b>，Query 仅携带 <c>access_token</c>。
    /// 产出的区域 <c>id</c>（districtid）是 <see cref="SearchMapPoiAsync"/> 与
    /// <see cref="CreateMapPoiAsync"/>（<c>districtid</c>）的取值来源。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Get("/wxa/get_district")]
    Task<MpDistrictResponse> GetDistrictListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 搜索门店地图信息。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_poilistsearch.html"/>
    /// （官方接口英文名 <c>poilistsearch</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">搜索请求（<c>districtid</c> + <c>keyword</c>，均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>地图点位列表（<c>data.item[]</c>；<c>sosomap_poi_uid</c> 即 <c>add_store</c> 的 <c>map_poi_id</c> 来源）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体见 <see cref="MpMapPoiSearchRequest"/>。</para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/wxa/search_map_poi")]
    Task<MpMapPoiSearchResponse> SearchMapPoiAsync(
        [Body] MpMapPoiSearchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增门店。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_addentityshop.html"/>
    /// （官方接口英文名 <c>addentityshop</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">新增请求（<c>map_poi_id</c> 取自 <see cref="SearchMapPoiAsync"/>；见 <see cref="MpAddStoreRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审核单 id（<c>data.audit_id</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体见 <see cref="MpAddStoreRequest"/>；审核结果经 <see cref="GetMerchantAuditInfoAsync"/> 查询。</para>
    /// <para>
    /// 官方错误码：<c>40001</c> / <c>85038</c>（store has added）/ <c>85039</c> / <c>85040</c>（已被绑定）/
    /// <c>85041</c>（credential has used）/ <c>85042</c>（nearby reach limit，附近地点添加数量达上限）/
    /// <c>85054</c>（poi_id is null）/ <c>85055</c>（map_poi_id is invalid）/ <c>85056</c>（mediaid is invalid）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/wxa/add_store")]
    Task<MpAddStoreResponse> AddStoreAsync(
        [Body] MpAddStoreRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取门店详情。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_newgetpoi.html"/>
    /// （官方接口英文名 <c>newgetpoi</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">查询请求（<c>poi_id</c>；与 <see cref="DeleteStoreAsync"/> 字段集一致 ⇒ 共用 <see cref="MpStorePoiRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>门店业务信息（<c>business.base_info</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体 <c>{"poi_id": …}</c>。
    /// <b>官方文档缺陷（照录）</b>：官方「返回参数」表仅列 <c>errcode</c>/<c>errmsg</c>，
    /// 而返回示例返回完整 <c>business.base_info</c> 树 ⇒ SDK 按「存在性冲突取并集」按示例建模。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/wxa/get_store_info")]
    Task<MpStoreInfoResponse> GetStoreInfoAsync(
        [Body] MpStorePoiRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取门店列表。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_newgetpoilist.html"/>
    /// （官方接口英文名 <c>newgetpoilist</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">分页请求（<c>offset</c> 从 0 起 + <c>limit</c>；官方未给出 limit 上限）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>门店列表（<c>business_list[]</c>）+ 门店总数（<c>total_count</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpStoreListRequest"/>。
    /// <b>官方文档缺陷（照录）</b>：字段表把 <c>base_info</c> 的子字段列为 <c>business_list[]</c> 平级字段，
    /// 而返回示例包裹在 <c>base_info</c> 内 ⇒ SDK 按「嵌套结构冲突取示例」建模为 <c>base_info</c> 嵌套。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/wxa/get_store_list")]
    Task<MpStoreListResponse> GetStoreListAsync(
        [Body] MpStoreListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除门店。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_newdelpoi.html"/>
    /// （官方接口英文名 <c>newdelpoi</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">删除请求（<c>poi_id</c>；与 <see cref="GetStoreInfoAsync"/> 字段集一致 ⇒ 共用 <see cref="MpStorePoiRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体 <c>{"poi_id": …}</c>。
    /// 官方文档缺陷（照录）：<c>poi_id</c> 的字段说明为「为门店小程序<b>添加</b>门店，审核成功后返回的门店 id」
    /// ——沿用自新增门店页，与「删除」语义不符。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/wxa/del_store")]
    Task<MpResponse> DeleteStoreAsync(
        [Body] MpStorePoiRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新门店信息。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_newupdatepoi.html"/>
    /// （官方接口英文名 <c>newupdatepoi</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">更新请求（<c>poi_id</c>/<c>pic_list</c>/<c>contract_phone</c>/<c>hour</c>/<c>card_id</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否需审核与审核单 id（<c>data.has_audit_id</c> / <c>data.audit_id</c>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpUpdateStoreRequest"/>。
    /// 官方<b>未提供</b>任何资质 / 名称 / 地址类更新字段（照录）。
    /// </para>
    /// <para>
    /// 官方错误码：<c>-1</c> / <c>40001</c> / <c>40097</c>（invalid args）/ <c>65115</c>（poi_id is not exist）/
    /// <c>65118</c>（store status is invalid，该门店状态不允许更新）/ <c>85053</c>（please apply merchant first）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/wxa/update_store")]
    Task<MpUpdateStoreResponse> UpdateStoreAsync(
        [Body] MpUpdateStoreRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 在地图中创建门店。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_createnewpoid.html"/>
    /// （官方接口英文名 <c>createnewpoid</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">创建请求（14 字段，见 <see cref="MpCreateMapPoiRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>审核单 id（<c>data.base_id</c>）与 <c>data.rich_id</c>（官方说明为 <c>-</c>，语义未定义）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体见 <see cref="MpCreateMapPoiRequest"/>。</para>
    /// <para>
    /// 官方文档缺陷（照录）：返回示例使用 <c>error: null</c>（未定义字段，与 <c>errcode</c> 体系不统一）；
    /// <c>poi_id</c> 标必填但说明为「迁移门店才必须填」。
    /// </para>
    /// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/wxa/create_map_poi")]
    Task<MpCreateMapPoiResponse> CreateMapPoiAsync(
        [Body] MpCreateMapPoiRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 门店小程序主体审核状态（官方 <c>get_merchant_audit_info</c> 的 <c>data.status</c> 取值）。
/// </summary>
public static class MpMerchantAuditStatuses
{
    /// <summary>未提交审核（官方 0）。</summary>
    public const int NotSubmitted = 0;

    /// <summary>审核成功（官方 1）。</summary>
    public const int Approved = 1;

    /// <summary>审核中（官方 2）。</summary>
    public const int Auditing = 2;

    /// <summary>审核失败（官方 3；此时 <c>reason</c> 给出失败原因）。</summary>
    public const int Rejected = 3;

    /// <summary>管理员拒绝（官方 4；此时 <c>reason</c> 给出失败原因）。</summary>
    public const int AdminRejected = 4;
}

/// <summary>
/// 门店审核结果（官方 <c>get_store_info</c> / <c>get_store_list</c> 的 <c>base_info.status</c> 取值）。
/// </summary>
/// <remarks>
/// 官方原文：<c>1</c>-审核通过 / <c>2</c>-审核中 / <c>3</c>-审核失败。注意与
/// <see cref="MpMerchantAuditStatuses"/>（主体审核，多「未提交 / 管理员拒绝」两态）<b>不是同一枚举</b>，勿混用。
/// </remarks>
public static class MpStoreStatuses
{
    /// <summary>审核通过（官方 1）。</summary>
    public const int Approved = 1;

    /// <summary>审核中（官方 2）。</summary>
    public const int Auditing = 2;

    /// <summary>审核失败（官方 3）。</summary>
    public const int Rejected = 3;
}

/// <summary>
/// 类目敏感类型（官方 <c>get_merchant_category</c> 的 <c>sensitive_type</c> 取值）。
/// </summary>
public static class MpStoreCategorySensitiveTypes
{
    /// <summary>不用特殊处理（官方 0）。</summary>
    public const int Normal = 0;

    /// <summary>创建该类目的门店小程序时需添加相关证件（官方 1）。</summary>
    public const int QualificationRequired = 1;
}
