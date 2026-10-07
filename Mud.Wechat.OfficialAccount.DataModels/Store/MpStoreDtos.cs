// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

// ---------------------------------------------------------------- 拉取门店小程序类目（GET /wxa/get_merchant_category）

/// <summary>
/// 拉取门店小程序类目（<c>GET /wxa/get_merchant_category</c>）响应。
/// </summary>
/// <remarks>
/// <para>官方字段表（逐页核验 2026-10-07）：<c>data.all_category_info.categories[]</c>。</para>
/// <para>官方错误码：<c>-1</c> / <c>40001</c>（本页错误码表仅此两行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpMerchantCategoryResponse : MpResponse
{
    /// <summary>获取或设置类目数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public MpMerchantCategoryData? Data { get; set; }
}

/// <summary>类目数据（官方 <c>data</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpMerchantCategoryData
{
    /// <summary>获取或设置类目信息（官方 <c>all_category_info</c>）。</summary>
    [JsonPropertyName("all_category_info")]
    public MpMerchantCategoryInfo? AllCategoryInfo { get; set; }
}

/// <summary>类目信息（官方 <c>data.all_category_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpMerchantCategoryInfo
{
    /// <summary>获取或设置类目列表（官方 <c>categories</c>）。</summary>
    [JsonPropertyName("categories")]
    public List<MpMerchantCategory>? Categories { get; set; }
}

/// <summary>
/// 门店小程序类目（官方 <c>data.all_category_info.categories[]</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表仅列 <c>id</c> / <c>level</c> / <c>sensitive_type</c>；官方返回示例另出现
/// <c>name</c> / <c>children</c> / <c>father</c> / <c>qualify</c> / <c>scene</c>。
/// 按「字段存在性冲突取并集」：纳入示例中<b>形态明确</b>的 <c>name</c>（类目名）与
/// <c>children</c>（同类目自引用数组，用于树形展开）；<c>father</c>（父类目，可由 <c>children</c>
/// 递归覆盖，冗余不建模）、<c>qualify</c>（子字段 <c>exter_list</c>/<c>inner_list</c> 元素形态未定义）、
/// <c>scene</c>（形态未定义）<b>不建模</b>（照录官方缺陷）。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：字段说明称 <c>level</c>「一级或者二级类目」，但示例根节点出现
/// <c>id:0, name:"root", level:0</c>（第三级层级描述）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpMerchantCategory
{
    /// <summary>获取或设置类目 id（官方 <c>id</c>）。</summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>获取或设置类目级别（官方 <c>level</c>；官方说明「一级或者二级类目」，示例另现 0，矛盾照录）。</summary>
    [JsonPropertyName("level")]
    public int? Level { get; set; }

    /// <summary>获取或设置敏感类型（官方 <c>sensitive_type</c>；<c>0</c> 不用特殊处理 / <c>1</c> 创建该类目的门店小程序时需添加相关证件）。</summary>
    [JsonPropertyName("sensitive_type")]
    public int? SensitiveType { get; set; }

    /// <summary>获取或设置类目名称（官方返回示例字段 <c>name</c>；官方字段表未收录，照录）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置子类目（官方返回示例字段 <c>children</c>；官方字段表未收录，照录）。</summary>
    [JsonPropertyName("children")]
    public List<MpMerchantCategory>? Children { get; set; }
}

// ---------------------------------------------------------------- 创建 / 修改门店小程序主体（POST /wxa/apply_merchant、/wxa/modify_merchant）

/// <summary>
/// 创建门店小程序（<c>POST /wxa/apply_merchant</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>first_catid</c> / <c>second_catid</c> /
/// <c>headimg_mediaid</c> / <c>nickname</c> / <c>intro</c> / <c>qualification_list</c> 为必填；
/// <c>org_code</c> / <c>other_files</c> 为非必填（官方错误码 <c>85024</c>「需要补充资料」表明
/// 特定场景下二者必填——必填性矛盾照录，SDK 不本地拦截）。
/// </para>
/// <para>
/// <b>开放面极窄（官方错误码 43104 原文，勿弱化）</b>：「this appid does not have permission__无调用权限，
/// <b>仅开放给电商类目（一级类目：电商平台、商家自营、跨境电商）</b>」——非电商类目主体调用本域即返回
/// <c>43104</c>（SDK 不做本地类目闸，由官方错误码表达）。
/// </para>
/// <para>
/// 官方原文约束：<c>nickname</c> 长度 <b>4~30 字符</b>（中文算两个字符），不得含特殊字符及「微信」等保留字。
/// </para>
/// <para>
/// SDK 调用前置（官方错误码表达，SDK 不编排）：需先经 <c>get_merchant_category</c> 取类目 id；
/// 需公众号管理员确认后进入审核流程（结果经 <c>get_merchant_audit_info</c> 查询）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpApplyMerchantRequest
{
    /// <summary>获取或设置一级类目 id（官方 <c>first_catid</c>，必填）。</summary>
    [JsonPropertyName("first_catid")]
    public int FirstCategoryId { get; set; }

    /// <summary>获取或设置二级类目 id（官方 <c>second_catid</c>，必填）。</summary>
    [JsonPropertyName("second_catid")]
    public int SecondCategoryId { get; set; }

    /// <summary>获取或设置头像临时素材 mediaid（官方 <c>headimg_mediaid</c>，必填；支持 jpg、png）。</summary>
    [JsonPropertyName("headimg_mediaid")]
    public string HeadImageMediaId { get; set; } = string.Empty;

    /// <summary>获取或设置门店小程序昵称（官方 <c>nickname</c>，必填；长度 4~30 字符，中文算两个字符）。</summary>
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = string.Empty;

    /// <summary>获取或设置门店小程序介绍（官方 <c>intro</c>，必填）。</summary>
    [JsonPropertyName("intro")]
    public string Intro { get; set; } = string.Empty;

    /// <summary>获取或设置类目相关证件（官方 <c>qualification_list</c>，必填；临时素材 mediaid）。</summary>
    [JsonPropertyName("qualification_list")]
    public string QualificationList { get; set; } = string.Empty;

    /// <summary>获取或设置营业执照或组织代码证（官方 <c>org_code</c>；临时素材 mediaid；官方错误码 85024 表明特定场景必填）。</summary>
    [JsonPropertyName("org_code")]
    public string? OrgCode { get; set; }

    /// <summary>获取或设置补充材料（官方 <c>other_files</c>；临时素材 mediaid；官方错误码 85024 表明特定场景必填）。</summary>
    [JsonPropertyName("other_files")]
    public string? OtherFiles { get; set; }
}

/// <summary>
/// 修改门店小程序信息（<c>POST /wxa/modify_merchant</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>headimg_mediaid</c> / <c>intro</c>；两字段均标「必填」但说明为「不改可传空值」
/// ——必填性与可空语义矛盾（照录，SDK 以可空承载）。
/// </para>
/// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpModifyMerchantRequest
{
    /// <summary>获取或设置头像临时素材 mediaid（官方 <c>headimg_mediaid</c>；不改可传空值）。</summary>
    [JsonPropertyName("headimg_mediaid")]
    public string? HeadImageMediaId { get; set; }

    /// <summary>获取或设置门店小程序介绍（官方 <c>intro</c>；不改可传空值）。</summary>
    [JsonPropertyName("intro")]
    public string? Intro { get; set; }
}

// ---------------------------------------------------------------- 获取门店小程序审核结果（POST /wxa/get_merchant_audit_info）

/// <summary>
/// 获取门店小程序审核结果（<c>POST /wxa/get_merchant_audit_info</c>）请求体。
/// </summary>
/// <remarks>
/// <b>方法核验裁决（官方三处矛盾，勿弱化）</b>：官方「调用方式」章标 <b>GET</b>，但「请求参数」章
/// 将 <c>audit_id</c> 定义为 <b>REQUEST PAYLOAD（请求体）</b>且标必填，而请求示例为 <c>{}</c>（空对象）
/// ——三处互相矛盾。SDK 裁决为 <b>POST + 请求体</b>，依据：①「请求参数」章的<b>参数位置</b>说明（请求体）
/// 是参数归属的权威表述；② HTTP GET <b>不应携带请求体</b>（RFC 7231 语义，且生成管线不产出 GET body），
/// 若坚持 GET 则 <c>audit_id</c> 无处安放；③ 与同域其余 merchant / store 端点的形态一致。
/// 官方 GET 标注照录（方案 F8 台账）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpMerchantAuditInfoRequest
{
    /// <summary>获取或设置审核单 id（官方 <c>audit_id</c>，必填）。</summary>
    [JsonPropertyName("audit_id")]
    public int AuditId { get; set; }
}

/// <summary>
/// 获取门店小程序审核结果（<c>POST /wxa/get_merchant_audit_info</c>）响应。
/// </summary>
/// <remarks>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpMerchantAuditInfoResponse : MpResponse
{
    /// <summary>获取或设置审核结果（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public MpMerchantAuditResult? Data { get; set; }
}

/// <summary>门店小程序审核结果（官方 <c>data</c>）。</summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>audit_id</c> / <c>status</c>（<c>0</c> 未提交审核 / <c>1</c> 审核成功 / <c>2</c> 审核中 /
/// <c>3</c> 审核失败 / <c>4</c> 管理员拒绝）/ <c>reason</c>（审核状态为 3 或 4 时列出审核失败的原因）。
/// </para>
/// <para>
/// <b>类型冲突口径（标量取官方返回示例）</b>：官方字段表标 <c>reason</c> 为 <c>number</c>，
/// 但返回示例为<b>字符串</b>（<c>""</c>）；「审核失败的原因」语义上亦为可读文本。
/// 按本 SDK 的冲突处置规则「<b>标量类型冲突取官方返回示例</b>」，建模为 <c>string</c>
/// （若按 <c>number</c> 建模则在官方示例形态下必然反序列化失败——字段表口径被示例直接证伪）。
/// </para>
/// <para>官方文档缺陷：<c>audit_id</c> 同时出现在请求参数与返回 <c>data</c> 内，未说明两者是否恒等（照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpMerchantAuditResult
{
    /// <summary>获取或设置审核单 id（官方 <c>audit_id</c>）。</summary>
    [JsonPropertyName("audit_id")]
    public int? AuditId { get; set; }

    /// <summary>获取或设置审核状态（官方 <c>status</c>；取值见 <c>MpMerchantAuditStatuses</c>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置审核失败原因（官方 <c>reason</c>；仅 <c>status</c> 为 3 / 4 时返回；字段表标 number、示例为字符串，取示例口径）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

// ---------------------------------------------------------------- 获取省市区信息（GET /wxa/get_district）

/// <summary>
/// 获取省市区信息（<c>GET /wxa/get_district</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>status</c>（状态码）/ <c>message</c>（状态描述）/ <c>data_version</c>（数据版本）/
/// <c>result</c>（数据内容，<b>二维数组</b>，分别代表省、市、区信息）。
/// </para>
/// <para>
/// <b>继承 <see cref="MpResponse"/> 的理由（超集）</b>：成功响应<b>不含</b> <c>errcode</c>/<c>errmsg</c>
/// （仅 status/message），但官方错误码表列 <c>40001</c> ⇒ 失败时以 <c>errcode</c> 表达；两个面各自成立，
/// 故以超集承载（成功时 <c>IsSuccess</c> 缺省为真）。
/// </para>
/// <para>官方文档缺陷（照录）：<c>status</c> 为 <c>0</c> 却称「状态码」，未给出成功/失败取值定义。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpDistrictResponse : MpResponse
{
    /// <summary>获取或设置状态码（官方 <c>status</c>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置状态描述（官方 <c>message</c>）。</summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>获取或设置数据版本（官方 <c>data_version</c>）。</summary>
    [JsonPropertyName("data_version")]
    public string? DataVersion { get; set; }

    /// <summary>获取或设置数据内容（官方 <c>result</c>，<b>二维数组</b>：外层省 / 市 / 区三级，内层区域对象）。</summary>
    [JsonPropertyName("result")]
    public List<List<MpDistrictInfo>>? Result { get; set; }
}

/// <summary>
/// 区域对象（官方 <c>result</c> 内层元素）。
/// </summary>
/// <remarks>
/// 官方文档缺陷（照录）：① 返回示例的第三层（区级）仅有 <c>id</c>/<c>fullname</c>/<c>location</c>，
/// 缺少字段表声明的 <c>name</c>/<c>pinyin</c>/<c>cidx</c>；② <c>cidx</c> 说明为「下属地区所有 id」，
/// 但示例省级仅 2 个值，与「所有」表述不符。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpDistrictInfo
{
    /// <summary>获取或设置区域 id（官方 <c>id</c>，也叫 districtid）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>获取或设置省市区简要名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置省市区完整名称（官方 <c>fullname</c>）。</summary>
    [JsonPropertyName("fullname")]
    public string? FullName { get; set; }

    /// <summary>获取或设置省市区拼音列表（官方 <c>pinyin</c>）。</summary>
    [JsonPropertyName("pinyin")]
    public List<string>? Pinyin { get; set; }

    /// <summary>获取或设置坐标（官方 <c>location</c>）。</summary>
    [JsonPropertyName("location")]
    public MpDistrictLocation? Location { get; set; }

    /// <summary>获取或设置下属地区 id 列表（官方 <c>cidx</c>；可通过此 id 获取下属地区）。</summary>
    [JsonPropertyName("cidx")]
    public List<int>? ChildIndexes { get; set; }
}

/// <summary>
/// 区域坐标（官方 <c>location</c>）。
/// </summary>
/// <remarks>
/// <b>类型冲突口径（标量取官方返回示例）</b>：官方字段表标 <c>lat</c>/<c>lng</c> 为 <c>string</c>，
/// 但返回示例为<b>数字</b>；本域同类字段在 <c>search_map_poi</c> 页字段表即标 <c>number</c>
/// ⇒ 字段表类型列在本域系统性失真。按「标量类型冲突取官方返回示例」建模为 <c>double</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpDistrictLocation
{
    /// <summary>获取或设置纬度（官方 <c>lat</c>；字段表标 string、示例为数字，取示例口径）。</summary>
    [JsonPropertyName("lat")]
    public double? Latitude { get; set; }

    /// <summary>获取或设置经度（官方 <c>lng</c>；字段表标 string、示例为数字，取示例口径）。</summary>
    [JsonPropertyName("lng")]
    public double? Longitude { get; set; }
}

// ---------------------------------------------------------------- 搜索门店地图信息（POST /wxa/search_map_poi）

/// <summary>
/// 搜索门店地图信息（<c>POST /wxa/search_map_poi</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：<c>districtid</c>（对应 <c>get_district</c> 返回的 <c>id</c> 字段）/ <c>keyword</c>（搜索关键词）。
/// 官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpMapPoiSearchRequest
{
    /// <summary>获取或设置省市区 id（官方 <c>districtid</c>，必填；取自 <c>get_district</c> 的区域 <c>id</c>）。</summary>
    [JsonPropertyName("districtid")]
    public int DistrictId { get; set; }

    /// <summary>获取或设置搜索关键词（官方 <c>keyword</c>，必填）。</summary>
    [JsonPropertyName("keyword")]
    public string Keyword { get; set; } = string.Empty;
}

/// <summary>搜索门店地图信息（<c>POST /wxa/search_map_poi</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpMapPoiSearchResponse : MpResponse
{
    /// <summary>获取或设置地图信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public MpMapPoiSearchData? Data { get; set; }
}

/// <summary>地图信息（官方 <c>data</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpMapPoiSearchData
{
    /// <summary>获取或设置信息数组（官方 <c>item</c>）。</summary>
    [JsonPropertyName("item")]
    public List<MpMapPoiItem>? Items { get; set; }
}

/// <summary>
/// 地图点位（官方 <c>data.item[]</c>）。
/// </summary>
/// <remarks>
/// 官方文档缺陷（照录）：<c>pic_urls</c> / <c>card_id_list</c> 仅标注为 array、未给出元素类型
/// （SDK 以 <c>List&lt;string&gt;</c> 承载）；<c>data_supply</c> 语义未定义（仅标注「地图数据」）。
/// <b><c>sosomap_poi_uid</c> 是 <c>add_store</c> 的 <c>map_poi_id</c> 取值来源</b>（官方原文）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpMapPoiItem
{
    /// <summary>获取或设置门店名称（官方 <c>branch_name</c>）。</summary>
    [JsonPropertyName("branch_name")]
    public string? BranchName { get; set; }

    /// <summary>获取或设置地址描述（官方 <c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>获取或设置经度（官方 <c>longitude</c>，number）。</summary>
    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    /// <summary>获取或设置纬度（官方 <c>latitude</c>，number）。</summary>
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    /// <summary>获取或设置电话号码（官方 <c>telephone</c>）。</summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>获取或设置类目（官方 <c>category</c>）。</summary>
    [JsonPropertyName("category")]
    public string? Category { get; set; }

    /// <summary>获取或设置地图点位 id（官方 <c>sosomap_poi_uid</c>；<c>add_store</c> 的 <c>map_poi_id</c> 即取此值）。</summary>
    [JsonPropertyName("sosomap_poi_uid")]
    public string? SosoMapPoiUid { get; set; }

    /// <summary>获取或设置地图数据（官方 <c>data_supply</c>；官方仅标注「地图数据」，语义未定义，照录）。</summary>
    [JsonPropertyName("data_supply")]
    public int? DataSupply { get; set; }

    /// <summary>获取或设置门店图片列表（官方 <c>pic_urls</c>；元素类型官方未给出，以字符串承载）。</summary>
    [JsonPropertyName("pic_urls")]
    public List<string>? PictureUrls { get; set; }

    /// <summary>获取或设置门店相应卡券列表（官方 <c>card_id_list</c>；元素类型官方未给出，以字符串承载）。</summary>
    [JsonPropertyName("card_id_list")]
    public List<string>? CardIdList { get; set; }
}

// ---------------------------------------------------------------- 门店 CRUD（add_store / get_store_info / get_store_list / del_store / update_store）

/// <summary>
/// 门店图片列表（官方 <c>pic_list</c> 表单值形态 <c>{"list":[…]}</c> 的承载）。
/// </summary>
/// <remarks>
/// <para>
/// 官方原文：<c>pic_list</c> 类型为 <c>string</c>，是「一个 <b>json 字符串</b>」，官方示例结构为
/// <c>{"list":[图片url]}</c>。字段本身以字符串承载（<see cref="MpAddStoreRequest.PictureList"/> /
/// <see cref="MpUpdateStoreRequest.PictureList"/>），本 DTO 用于生成该字符串
/// （与素材域 <c>MpMaterialDescription</c> 同形先例）：
/// <c>JsonSerializer.Serialize(pics, StoreJsonContext.Default.MpStorePictureList)</c>。
/// </para>
/// <para>SDK 不做图片数量/格式拦截（官方未给出数值上限）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpStorePictureList
{
    /// <summary>获取或设置图片 URL 列表（官方 JSON 字符串内的 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public List<string> List { get; set; } = new List<string>();
}

/// <summary>
/// 新增门店（<c>POST /wxa/add_store</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>map_poi_id</c> / <c>pic_list</c> / <c>contract_phone</c> /
/// <c>hour</c> / <c>credential</c> / <c>card_id</c> 标必填；<c>company_name</c> / <c>qualification_list</c> /
/// <c>poi_id</c> 标非必填。
/// </para>
/// <para>
/// <b>官方必填性矛盾（照录，SDK 以可空承载、不本地拦截）</b>：① <c>card_id</c> 标必填但说明
/// 「如果不需要添加卡券，该参数可为空」；② <c>poi_id</c> 标非必填但说明「如果是迁移场景必须填」；
/// ③ <c>company_name</c> 标非必填但说明「不复用公众号主体则填具体主体名字」⇒ 条件必填。
/// </para>
/// <para>
/// <b>卡券约束（官方原文，勿弱化）</b>：<c>card_id</c>「目前仅开放支持<b>会员卡、买单、刷卡支付券</b>，
/// <b>不支持自定义 code</b>，需先去公众平台卡券后台创建 cardid」——本域只<b>透传</b>卡券 id，
/// 不建模卡券域、不消费卡券 <c>api_ticket</c>。
/// </para>
/// <para>
/// <b><c>qualification_list</c> 类型冲突口径</b>：官方字段表标 <c>array</c>，但说明为「支持 <b>0~5 个</b>
/// mediaid，例如 mediaid1 或 mediaid2」、示例为<b>单个字符串</b> ⇒ 按「容器形态冲突取字段表」建模为
/// <c>List&lt;string&gt;</c>（字段表为 schema 声明口径）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpAddStoreRequest
{
    /// <summary>获取或设置腾讯地图点位 id（官方 <c>map_poi_id</c>，必填；即 <c>search_map_poi</c> 返回的 <c>sosomap_poi_uid</c>）。</summary>
    [JsonPropertyName("map_poi_id")]
    public string MapPoiId { get; set; } = string.Empty;

    /// <summary>获取或设置门店图片（官方 <c>pic_list</c>，必填；一个 JSON 字符串，见 <see cref="MpStorePictureList"/>）。</summary>
    [JsonPropertyName("pic_list")]
    public string PictureList { get; set; } = string.Empty;

    /// <summary>获取或设置联系电话（官方 <c>contract_phone</c>，必填）。</summary>
    [JsonPropertyName("contract_phone")]
    public string ContractPhone { get; set; } = string.Empty;

    /// <summary>获取或设置营业时间（官方 <c>hour</c>，必填；格式 <c>11:11-12:12</c>）。</summary>
    [JsonPropertyName("hour")]
    public string Hour { get; set; } = string.Empty;

    /// <summary>获取或设置经营资质证件号（官方 <c>credential</c>，必填；唯一性由官方 85041 表达）。</summary>
    [JsonPropertyName("credential")]
    public string Credential { get; set; } = string.Empty;

    /// <summary>获取或设置主体名字（官方 <c>company_name</c>；复用公众号主体则为空，不复用则填具体主体名字）。</summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

    /// <summary>获取或设置卡券 id（官方 <c>card_id</c>；不需要添加卡券时可为空；仅支持会员卡 / 买单 / 刷卡支付券，不支持自定义 code）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置相关证明材料（官方 <c>qualification_list</c>；临时素材 mediaid，支持 0~5 个；不复用公众号主体时才需要填）。</summary>
    [JsonPropertyName("qualification_list")]
    public List<string>? QualificationList { get; set; }

    /// <summary>获取或设置门店迁移用 poi_id（官方 <c>poi_id</c>；从门店管理迁移门店到门店小程序时需要填）。</summary>
    [JsonPropertyName("poi_id")]
    public string? PoiId { get; set; }
}

/// <summary>
/// 新增门店（<c>POST /wxa/add_store</c>）响应。
/// </summary>
/// <remarks>
/// 官方错误码：<c>40001</c> / <c>85038</c>（store has added）/ <c>85039</c> / <c>85040</c> /
/// <c>85041</c>（credential has used）/ <c>85042</c>（nearby reach limit）/ <c>85054</c>（poi_id is null）/
/// <c>85055</c>（map_poi_id is invalid）/ <c>85056</c>（mediaid is invalid）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpAddStoreResponse : MpResponse
{
    /// <summary>获取或设置新增信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public MpStoreAuditResult? Data { get; set; }
}

/// <summary>门店审核单信息（官方 <c>data</c>；<c>add_store</c> 响应承载）。</summary>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpStoreAuditResult
{
    /// <summary>获取或设置审核单 id（官方 <c>audit_id</c>）。</summary>
    [JsonPropertyName("audit_id")]
    public int? AuditId { get; set; }
}

/// <summary>
/// 按 <c>poi_id</c> 寻址门店的共用请求体（官方 <c>get_store_info</c> 与 <c>del_store</c> 请求体字段集一致 ⇒ 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpStorePoiRequest
{
    /// <summary>获取或设置门店 id（官方 <c>poi_id</c>；为门店小程序添加门店审核成功后返回的门店 id）。</summary>
    [JsonPropertyName("poi_id")]
    public string PoiId { get; set; } = string.Empty;
}

/// <summary>
/// 获取门店详情（<c>POST /wxa/get_store_info</c>）响应。
/// </summary>
/// <remarks>
/// <b>官方文档缺陷（照录，SDK 按「字段存在性冲突取并集」超集建模）</b>：官方「返回参数」表
/// <b>仅列 <c>errcode</c>/<c>errmsg</c> 两字段</b>，但「返回示例」实际返回完整的
/// <c>business.base_info</c> 嵌套对象（14 字段 + <c>photo_list</c> 数组），字段表与示例严重不一致；
/// SDK 按示例建模（否则该端点无可消费的响应体）。官方错误码：<c>40001</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpStoreInfoResponse : MpResponse
{
    /// <summary>获取或设置门店业务信息（官方 <c>business</c>，仅见返回示例、字段表未收录）。</summary>
    [JsonPropertyName("business")]
    public MpStoreBusiness? Business { get; set; }
}

/// <summary>门店业务信息（官方 <c>business</c>，仅见返回示例）。</summary>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpStoreBusiness
{
    /// <summary>获取或设置门店基本信息（官方 <c>base_info</c>，仅见返回示例）。</summary>
    [JsonPropertyName("base_info")]
    public MpStoreBaseInfo? BaseInfo { get; set; }
}

/// <summary>
/// 门店基本信息（官方 <c>base_info</c>；<c>get_store_info</c> 的 <c>business.base_info</c> 与
/// <c>get_store_list</c> 的 <c>business_list[].base_info</c> 字段集一致 ⇒ 共用）。
/// </summary>
/// <remarks>
/// <b>官方文档缺陷（照录）</b>：① <c>get_store_list</c> 页把本对象的字段直接列为
/// <c>business_list[]</c> 的<b>平级字段</b>，而返回示例实际包裹在 <c>base_info</c> 内 ——
/// 按本 SDK 的冲突处置规则「<b>嵌套结构冲突取官方返回示例</b>」（表把示例嵌套扁平化属表的抽象失真），
/// 建模为 <c>base_info</c> 嵌套；② <c>longitude</c>/<c>latitude</c> 字段表标 <c>string</c>、示例为
/// <b>数字</b> ⇒ 按「标量类型冲突取官方返回示例」建模为 <c>double</c>；③
/// <c>qualification_name</c> 字段说明为「营业执照的名称」，但示例值恒与 <c>business_name</c> 相同，照录。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpStoreBaseInfo
{
    /// <summary>获取或设置门店名称（官方 <c>business_name</c>）。</summary>
    [JsonPropertyName("business_name")]
    public string? BusinessName { get; set; }

    /// <summary>获取或设置详细地址（官方 <c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>获取或设置电话（官方 <c>telephone</c>；可多个，使用英文分号间隔）。</summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>获取或设置城市（官方 <c>city</c>）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>获取或设置省份（官方 <c>province</c>）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>获取或设置经度（官方 <c>longitude</c>；字段表标 string、示例为数字，取示例口径）。</summary>
    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    /// <summary>获取或设置纬度（官方 <c>latitude</c>；字段表标 string、示例为数字，取示例口径）。</summary>
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    /// <summary>获取或设置图片列表（官方 <c>photo_list</c>）。</summary>
    [JsonPropertyName("photo_list")]
    public List<MpStorePhoto>? PhotoList { get; set; }

    /// <summary>获取或设置门店开放时间（官方 <c>open_time</c>）。</summary>
    [JsonPropertyName("open_time")]
    public string? OpenTime { get; set; }

    /// <summary>获取或设置门店 id（官方 <c>poi_id</c>）。</summary>
    [JsonPropertyName("poi_id")]
    public string? PoiId { get; set; }

    /// <summary>获取或设置审核结果（官方 <c>status</c>；取值见 <c>MpStoreStatuses</c>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置区（官方 <c>district</c>；官方示例为占位符 <c>value</c>，照录）。</summary>
    [JsonPropertyName("district")]
    public string? District { get; set; }

    /// <summary>获取或设置营业执照号（官方 <c>qualification_num</c>）。</summary>
    [JsonPropertyName("qualification_num")]
    public string? QualificationNumber { get; set; }

    /// <summary>获取或设置营业执照的名称（官方 <c>qualification_name</c>；官方示例值恒与 <c>business_name</c> 相同，照录）。</summary>
    [JsonPropertyName("qualification_name")]
    public string? QualificationName { get; set; }
}

/// <summary>门店图片（官方 <c>photo_list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpStorePhoto
{
    /// <summary>获取或设置图片 URL（官方 <c>photo_url</c>）。</summary>
    [JsonPropertyName("photo_url")]
    public string? PhotoUrl { get; set; }
}

/// <summary>
/// 获取门店列表（<c>POST /wxa/get_store_list</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：<c>offset</c>（初始偏移位置，从 0 开始计数）/ <c>limit</c>（获取门店个数）。
/// 官方未给出 <c>limit</c> 数值上限（照录，SDK 不本地拦截）。官方错误码：<c>40001</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpStoreListRequest
{
    /// <summary>获取或设置初始偏移位置（官方 <c>offset</c>，必填；从 0 开始计数）。</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>获取或设置获取门店个数（官方 <c>limit</c>，必填；官方未给出上限数值）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}

/// <summary>获取门店列表（<c>POST /wxa/get_store_list</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpStoreListResponse : MpResponse
{
    /// <summary>获取或设置门店列表（官方 <c>business_list</c>）。</summary>
    [JsonPropertyName("business_list")]
    public List<MpStoreListItem>? BusinessList { get; set; }

    /// <summary>获取或设置门店总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }
}

/// <summary>
/// 门店列表条目（官方 <c>business_list[]</c>）。
/// </summary>
/// <remarks>
/// 官方文档缺陷（照录）：返回示例出现 <c>base_info.categories</c> 与 <c>base_info.qualification_list</c>
/// （示例中均为空数组），但字段表完全没有这两个字段 ⇒ 按「存在性冲突取并集」保留。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpStoreListItem
{
    /// <summary>获取或设置门店基本信息（官方 <c>base_info</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpStoreBaseInfo? BaseInfo { get; set; }

    /// <summary>获取或设置门店类目（官方返回示例字段 <c>base_info.categories</c>；字段表未收录，照录）。</summary>
    [JsonPropertyName("categories")]
    public List<string>? Categories { get; set; }

    /// <summary>获取或设置资质材料（官方返回示例字段 <c>base_info.qualification_list</c>；字段表未收录，照录）。</summary>
    [JsonPropertyName("qualification_list")]
    public List<string>? QualificationList { get; set; }
}

/// <summary>
/// 更新门店信息（<c>POST /wxa/update_store</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>poi_id</c> / <c>pic_list</c> / <c>contract_phone</c> / <c>hour</c> / <c>card_id</c>。
/// 官方<b>未提供</b>任何资质 / 名称 / 地址类字段（更新门店基础资料无对应参数，照录）。
/// <c>card_id</c> 标必填但说明「不需要添加卡券时该参数可为空」——必填性矛盾照录。
/// </para>
/// <para>
/// <b>官方文档缺陷（照录）</b>：<c>map_poi_id</c> 出现在官方请求示例中，但请求参数表与 Query 表中
/// <b>均未定义</b>该字段（无类型、无必填性、无说明）⇒ SDK 不建模该字段。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpUpdateStoreRequest
{
    /// <summary>获取或设置门店 id（官方 <c>poi_id</c>，必填）。</summary>
    [JsonPropertyName("poi_id")]
    public string PoiId { get; set; } = string.Empty;

    /// <summary>获取或设置门店图片（官方 <c>pic_list</c>，必填；一个 JSON 字符串，见 <see cref="MpStorePictureList"/>）。</summary>
    [JsonPropertyName("pic_list")]
    public string PictureList { get; set; } = string.Empty;

    /// <summary>获取或设置联系电话（官方 <c>contract_phone</c>，必填）。</summary>
    [JsonPropertyName("contract_phone")]
    public string ContractPhone { get; set; } = string.Empty;

    /// <summary>获取或设置营业时间（官方 <c>hour</c>，必填；格式 <c>11:11-12:12</c>）。</summary>
    [JsonPropertyName("hour")]
    public string Hour { get; set; } = string.Empty;

    /// <summary>获取或设置卡券 id（官方 <c>card_id</c>；不需要添加卡券时可为空；约束同 <c>add_store</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }
}

/// <summary>
/// 更新门店信息（<c>POST /wxa/update_store</c>）响应。
/// </summary>
/// <remarks>
/// 官方错误码：<c>-1</c> / <c>40001</c> / <c>40097</c>（invalid args）/ <c>65115</c>（poi_id is not exist）/
/// <c>65118</c>（store status is invalid，该门店状态不允许更新）/ <c>85053</c>（please apply merchant first）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpUpdateStoreResponse : MpResponse
{
    /// <summary>获取或设置更新信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public MpStoreUpdateResult? Data { get; set; }
}

/// <summary>门店更新结果（官方 <c>data</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpStoreUpdateResult
{
    /// <summary>获取或设置是否需要审核（官方 <c>has_audit_id</c>；<c>1</c> 需要 / <c>0</c> 不需要）。</summary>
    [JsonPropertyName("has_audit_id")]
    public int? HasAuditId { get; set; }

    /// <summary>获取或设置审核单 id（官方 <c>audit_id</c>）。</summary>
    [JsonPropertyName("audit_id")]
    public int? AuditId { get; set; }
}

// ---------------------------------------------------------------- 在地图中创建门店（POST /wxa/create_map_poi）

/// <summary>
/// 在地图中创建门店（<c>POST /wxa/create_map_poi</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表 14 项全部标必填；官方文档缺陷（照录）：<c>poi_id</c> 虽标必填，但说明为
/// 「如果是迁移门店，必须填 poi_id 字段」⇒ 实为条件必填。<c>introduct</c> 为官方字段原文
/// （疑为 <c>introduction</c> 的官方拼写，<b>不得「规范化」</b>）。
/// </para>
/// <para>
/// <c>longitude</c>/<c>latitude</c> 本页字段表标 <c>string</c> 且无返回示例可对照 ⇒ 无类型冲突，
/// 按字段表取 <c>string</c>（与 <c>get_district</c> / <c>get_store_list</c> 的示例数字口径不同——
/// 那两处属「标量类型冲突取示例」，此处无冲突故取表，矛盾根源在官方跨页不一致，照录）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpCreateMapPoiRequest
{
    /// <summary>获取或设置名字（官方 <c>name</c>，必填）。</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>获取或设置经度（官方 <c>longitude</c>，必填；字段表标 string）。</summary>
    [JsonPropertyName("longitude")]
    public string Longitude { get; set; } = string.Empty;

    /// <summary>获取或设置纬度（官方 <c>latitude</c>，必填；字段表标 string）。</summary>
    [JsonPropertyName("latitude")]
    public string Latitude { get; set; } = string.Empty;

    /// <summary>获取或设置省份（官方 <c>province</c>，必填）。</summary>
    [JsonPropertyName("province")]
    public string Province { get; set; } = string.Empty;

    /// <summary>获取或设置城市（官方 <c>city</c>，必填）。</summary>
    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    /// <summary>获取或设置区（官方 <c>district</c>，必填）。</summary>
    [JsonPropertyName("district")]
    public string District { get; set; } = string.Empty;

    /// <summary>获取或设置详细地址（官方 <c>address</c>，必填）。</summary>
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    /// <summary>获取或设置类目（官方 <c>category</c>，必填；如「美食:中餐厅」）。</summary>
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    /// <summary>获取或设置电话（官方 <c>telephone</c>，必填；可多个，使用英文分号间隔）。</summary>
    [JsonPropertyName("telephone")]
    public string Telephone { get; set; } = string.Empty;

    /// <summary>获取或设置门店图片 url（官方 <c>photo</c>，必填）。</summary>
    [JsonPropertyName("photo")]
    public string Photo { get; set; } = string.Empty;

    /// <summary>获取或设置营业执照 url（官方 <c>license</c>，必填）。</summary>
    [JsonPropertyName("license")]
    public string License { get; set; } = string.Empty;

    /// <summary>获取或设置介绍（官方 <c>introduct</c>，必填；官方字段原文拼写，不得规范化）。</summary>
    [JsonPropertyName("introduct")]
    public string Introduct { get; set; } = string.Empty;

    /// <summary>获取或设置省市区 id（官方 <c>districtid</c>，必填；腾讯地图拉取省市区信息接口返回的 id）。</summary>
    [JsonPropertyName("districtid")]
    public string DistrictId { get; set; } = string.Empty;

    /// <summary>获取或设置门店迁移用 poi_id（官方 <c>poi_id</c>；官方标必填但说明为迁移场景才必填，矛盾照录）。</summary>
    [JsonPropertyName("poi_id")]
    public string PoiId { get; set; } = string.Empty;
}

/// <summary>
/// 在地图中创建门店（<c>POST /wxa/create_map_poi</c>）响应。
/// </summary>
/// <remarks>
/// 官方文档缺陷（照录）：返回示例使用 <c>error: null</c> 且全文未定义 <c>error</c> 字段（与字段表的
/// <c>errcode</c>/<c>errmsg</c> 体系不统一）；<c>rich_id</c> 官方说明为 <c>-</c>（无字段含义描述）。
/// 官方错误码：<c>40001</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpCreateMapPoiResponse : MpResponse
{
    /// <summary>获取或设置数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public MpCreateMapPoiResult? Data { get; set; }
}

/// <summary>地图门店创建结果（官方 <c>data</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpCreateMapPoiResult
{
    /// <summary>获取或设置审核单 id（官方 <c>base_id</c>）。</summary>
    [JsonPropertyName("base_id")]
    public int? BaseId { get; set; }

    /// <summary>获取或设置 <c>rich_id</c>（官方字段说明为 <c>-</c>，语义未定义，照录）。</summary>
    [JsonPropertyName("rich_id")]
    public int? RichId { get; set; }
}
