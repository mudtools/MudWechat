// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

/// <summary>
/// OCR / 图像处理共用的坐标点（官方四角点的 <c>x</c>/<c>y</c>）。
/// </summary>
/// <remarks>
/// 官方字段表原文：<c>x</c> = x 坐标，<c>y</c> = y 坐标；四角点（left_top / right_top /
/// right_bottom / left_bottom）字段结构相同 ⇒ 共用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrPoint
{
    /// <summary>获取或设置 x 坐标（官方 <c>x</c>）。</summary>
    [JsonPropertyName("x")]
    public double? X { get; set; }

    /// <summary>获取或设置 y 坐标（官方 <c>y</c>）。</summary>
    [JsonPropertyName("y")]
    public double? Y { get; set; }
}

/// <summary>
/// OCR / 图像处理共用的矩形位置（官方四角点容器 <c>pos</c> / <c>cert_position</c>）。
/// </summary>
/// <remarks>
/// 官方字段表原文：<c>left_top</c> 左上角 / <c>right_top</c> 右上角 /
/// <c>right_bottom</c> 右下角 / <c>left_bottom</c> 左下角，四者字段结构相同。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrRect
{
    /// <summary>获取或设置左上角位置（官方 <c>left_top</c>）。</summary>
    [JsonPropertyName("left_top")]
    public MpOcrPoint? LeftTop { get; set; }

    /// <summary>获取或设置右上角位置（官方 <c>right_top</c>）。</summary>
    [JsonPropertyName("right_top")]
    public MpOcrPoint? RightTop { get; set; }

    /// <summary>获取或设置右下角位置（官方 <c>right_bottom</c>）。</summary>
    [JsonPropertyName("right_bottom")]
    public MpOcrPoint? RightBottom { get; set; }

    /// <summary>获取或设置左下角位置（官方 <c>left_bottom</c>）。</summary>
    [JsonPropertyName("left_bottom")]
    public MpOcrPoint? LeftBottom { get; set; }
}

/// <summary>
/// OCR / 图像处理共用的图片尺寸（官方 <c>img_size</c>）。
/// </summary>
/// <remarks>官方字段表原文：<c>w</c> = 宽度，<c>h</c> = 高度。</remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpImageSize
{
    /// <summary>获取或设置图片宽度（官方 <c>w</c>）。</summary>
    [JsonPropertyName("w")]
    public int? Width { get; set; }

    /// <summary>获取或设置图片高度（官方 <c>h</c>）。</summary>
    [JsonPropertyName("h")]
    public int? Height { get; set; }
}

/// <summary>
/// 身份证识别（<c>POST /cv/ocr/idcard</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>type</c>（正面 / 背面，取值为 <c>Front</c>/<c>Back</c>）/
/// <c>name</c> / <c>id</c> / <c>valid_date</c> / <c>addr</c> / <c>gender</c> / <c>nationality</c>。
/// 正反面返回字段不同（正面返回 name/id/addr/gender/nationality，背面返回 valid_date）。
/// </para>
/// <para>
/// 官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c>（invalid image url）/
/// <c>101001</c>（certificate not found）/<c>101002</c>（decode image failed，官方解决方案列写
/// 「图片大小超过限制，resp_type = 0: 2MB，resp_type = 1: 10MB」但全文未定义 <c>resp_type</c>）/
/// <c>101003</c>（not enough market quota，官方解决方案列留空，照录）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrIdCardResponse : MpResponse
{
    /// <summary>获取或设置正反面标志（官方 <c>type</c>，<c>Front</c> 正面 / <c>Back</c> 背面）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置姓名（官方 <c>name</c>，正面返回）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置身份证号（官方 <c>id</c>，正面返回）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>获取或设置有效期（官方 <c>valid_date</c>，背面返回）。</summary>
    [JsonPropertyName("valid_date")]
    public string? ValidDate { get; set; }

    /// <summary>获取或设置地址（官方 <c>addr</c>，正面返回）。</summary>
    [JsonPropertyName("addr")]
    public string? Address { get; set; }

    /// <summary>获取或设置性别（官方 <c>gender</c>，正面返回）。</summary>
    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    /// <summary>获取或设置民族（官方 <c>nationality</c>，正面返回）。</summary>
    [JsonPropertyName("nationality")]
    public string? Nationality { get; set; }
}

/// <summary>
/// 银行卡识别（<c>POST /cv/ocr/bankcard</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档矛盾（照录，SDK 以超集承载不漏数据）</b>：返回参数表报字段名为 <c>number</c>（银行卡号），
/// 但官方 CURL 与云函数两处返回示例实际输出的键为 <c>id</c>。SDK 同时保留
/// <see cref="Number"/> 与 <see cref="Id"/> 两个属性（按报文实际形态取值），不做归一化。
/// </para>
/// <para>
/// 官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c> / <c>101001</c> / <c>101003</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrBankCardResponse : MpResponse
{
    /// <summary>获取或设置银行卡号（官方字段表字段名 <c>number</c>；返回示例未使用该键）。</summary>
    [JsonPropertyName("number")]
    public string? Number { get; set; }

    /// <summary>获取或设置银行卡号（官方返回示例实际使用的键 <c>id</c>；字段表未列）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }
}

/// <summary>
/// 行驶证识别（<c>POST /cv/ocr/driving</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>plate_num</c> / <c>vehicle_type</c> / <c>owner</c> /
/// <c>addr</c> / <c>use_character</c> / <c>model</c> / <c>vin</c> / <c>engine_num</c> /
/// <c>register_date</c> / <c>issue_date</c> / <c>plate_num_b</c> / <c>record</c> /
/// <c>passengers_num</c> / <c>total_quality</c> / <c>prepare_quality</c>。
/// </para>
/// <para>
/// <b>官方文档缺陷（照录，SDK 按超集建模）</b>：官方返回示例额外出现
/// <c>overall_size</c>（外廓尺寸）/ <c>card_position_front</c> / <c>card_position_back</c> /
/// <c>img_size</c>，四者<b>未列入返回参数表</b>；<c>plate_num</c> 与 <c>plate_num_b</c> 的官方说明
/// 完全相同（均为「车牌号码」），未区分二者含义。
/// </para>
/// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c> / <c>101003</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrDrivingResponse : MpResponse
{
    /// <summary>获取或设置车牌号码（官方 <c>plate_num</c>）。</summary>
    [JsonPropertyName("plate_num")]
    public string? PlateNumber { get; set; }

    /// <summary>获取或设置车辆类型（官方 <c>vehicle_type</c>）。</summary>
    [JsonPropertyName("vehicle_type")]
    public string? VehicleType { get; set; }

    /// <summary>获取或设置所有人（官方 <c>owner</c>）。</summary>
    [JsonPropertyName("owner")]
    public string? Owner { get; set; }

    /// <summary>获取或设置住址（官方 <c>addr</c>）。</summary>
    [JsonPropertyName("addr")]
    public string? Address { get; set; }

    /// <summary>获取或设置使用性质（官方 <c>use_character</c>）。</summary>
    [JsonPropertyName("use_character")]
    public string? UseCharacter { get; set; }

    /// <summary>获取或设置品牌型号（官方 <c>model</c>）。</summary>
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    /// <summary>获取或设置车辆识别代号（官方 <c>vin</c>）。</summary>
    [JsonPropertyName("vin")]
    public string? Vin { get; set; }

    /// <summary>获取或设置发动机号码（官方 <c>engine_num</c>）。</summary>
    [JsonPropertyName("engine_num")]
    public string? EngineNumber { get; set; }

    /// <summary>获取或设置注册日期（官方 <c>register_date</c>）。</summary>
    [JsonPropertyName("register_date")]
    public string? RegisterDate { get; set; }

    /// <summary>获取或设置发证日期（官方 <c>issue_date</c>）。</summary>
    [JsonPropertyName("issue_date")]
    public string? IssueDate { get; set; }

    /// <summary>获取或设置车牌号码（官方 <c>plate_num_b</c>；官方说明与 <c>plate_num</c> 完全相同，照录）。</summary>
    [JsonPropertyName("plate_num_b")]
    public string? PlateNumberB { get; set; }

    /// <summary>获取或设置号牌（官方 <c>record</c>）。</summary>
    [JsonPropertyName("record")]
    public string? Record { get; set; }

    /// <summary>获取或设置核定载人数（官方 <c>passengers_num</c>）。</summary>
    [JsonPropertyName("passengers_num")]
    public string? PassengersNumber { get; set; }

    /// <summary>获取或设置总质量（官方 <c>total_quality</c>）。</summary>
    [JsonPropertyName("total_quality")]
    public string? TotalQuality { get; set; }

    /// <summary>获取或设置整备质量（官方 <c>prepare_quality</c>）。</summary>
    [JsonPropertyName("prepare_quality")]
    public string? PrepareQuality { get; set; }

    /// <summary>获取或设置外廓尺寸（官方返回示例字段 <c>overall_size</c>；官方字段表未收录，照录）。</summary>
    [JsonPropertyName("overall_size")]
    public string? OverallSize { get; set; }

    /// <summary>获取或设置卡片正面位置（官方返回示例字段 <c>card_position_front</c>；官方字段表未收录，照录）。</summary>
    [JsonPropertyName("card_position_front")]
    public MpOcrCardPosition? CardPositionFront { get; set; }

    /// <summary>获取或设置卡片反面位置（官方返回示例字段 <c>card_position_back</c>；官方字段表未收录，照录）。</summary>
    [JsonPropertyName("card_position_back")]
    public MpOcrCardPosition? CardPositionBack { get; set; }

    /// <summary>获取或设置图片尺寸（官方返回示例字段 <c>img_size</c>；官方字段表未收录，照录）。</summary>
    [JsonPropertyName("img_size")]
    public MpImageSize? ImageSize { get; set; }
}

/// <summary>
/// 卡片位置（官方返回示例字段 <c>card_position_front</c> / <c>card_position_back</c> 的承载；字段表未收录）。
/// </summary>
/// <remarks>官方返回示例形态为 <c>{ "pos": { left_top / right_top / right_bottom / left_bottom } }</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrCardPosition
{
    /// <summary>获取或设置四角点位置（官方 <c>pos</c>）。</summary>
    [JsonPropertyName("pos")]
    public MpOcrRect? Position { get; set; }
}

/// <summary>
/// 驾驶证识别（<c>POST /cv/ocr/drivinglicense</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>id_num</c> / <c>name</c> / <c>sex</c> / <c>address</c> /
/// <c>birth_date</c> / <c>issue_date</c> / <c>car_class</c> / <c>valid_from</c> / <c>valid_to</c> /
/// <c>official_seal</c>（官方说明原文误作「印章文构」，照录）。
/// </para>
/// <para>
/// <b>官方文档缺陷（照录）</b>：<c>nationality</c> 出现在官方返回示例中但字段表未收录；
/// 云函数返回示例使用驼峰命名（<c>birthDate</c>/<c>carClass</c>…），与 HTTPS 示例的下划线命名不一致
/// ——SDK 以 HTTPS 示例的下划线字段名为准。
/// </para>
/// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c> / <c>101003</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrDrivingLicenseResponse : MpResponse
{
    /// <summary>获取或设置证号（官方 <c>id_num</c>）。</summary>
    [JsonPropertyName("id_num")]
    public string? IdNumber { get; set; }

    /// <summary>获取或设置姓名（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置性别（官方 <c>sex</c>）。</summary>
    [JsonPropertyName("sex")]
    public string? Sex { get; set; }

    /// <summary>获取或设置国籍（官方返回示例字段 <c>nationality</c>；官方字段表未收录，照录）。</summary>
    [JsonPropertyName("nationality")]
    public string? Nationality { get; set; }

    /// <summary>获取或设置地址（官方 <c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>获取或设置出生日期（官方 <c>birth_date</c>）。</summary>
    [JsonPropertyName("birth_date")]
    public string? BirthDate { get; set; }

    /// <summary>获取或设置初次领证日期（官方 <c>issue_date</c>）。</summary>
    [JsonPropertyName("issue_date")]
    public string? IssueDate { get; set; }

    /// <summary>获取或设置准驾车型（官方 <c>car_class</c>）。</summary>
    [JsonPropertyName("car_class")]
    public string? CarClass { get; set; }

    /// <summary>获取或设置有效期限起始日（官方 <c>valid_from</c>）。</summary>
    [JsonPropertyName("valid_from")]
    public string? ValidFrom { get; set; }

    /// <summary>获取或设置有效期限终止日（官方 <c>valid_to</c>）。</summary>
    [JsonPropertyName("valid_to")]
    public string? ValidTo { get; set; }

    /// <summary>获取或设置印章文字（官方 <c>official_seal</c>；官方说明原文误作「印章文构」，照录）。</summary>
    [JsonPropertyName("official_seal")]
    public string? OfficialSeal { get; set; }
}

/// <summary>
/// 营业执照识别（<c>POST /cv/ocr/bizlicense</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>reg_num</c> / <c>serial</c> / <c>legal_representative</c> /
/// <c>enterprise_name</c> / <c>type_of_organization</c> / <c>address</c> / <c>type_of_enterprise</c> /
/// <c>business_scope</c> / <c>registered_capital</c> / <c>paid_in_capital</c> / <c>valid_period</c> /
/// <c>registered_date</c> / <c>cert_position</c>（营业执照位置）/ <c>img_size</c>（图片大小）。
/// </para>
/// <para>
/// 官方「注意事项」原文：「<b>返回字段仅包含当前营业执照图片中存在的字段，若对应字段不存在则不返回</b>」
/// ⇒ 各字段均为可空，不得以非空判定识别成功。
/// </para>
/// <para>
/// 官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c> / <c>101001</c> / <c>101002</c> / <c>101003</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrBizLicenseResponse : MpResponse
{
    /// <summary>获取或设置注册号（官方 <c>reg_num</c>）。</summary>
    [JsonPropertyName("reg_num")]
    public string? RegNumber { get; set; }

    /// <summary>获取或设置编号（官方 <c>serial</c>）。</summary>
    [JsonPropertyName("serial")]
    public string? Serial { get; set; }

    /// <summary>获取或设置法定代表人姓名（官方 <c>legal_representative</c>）。</summary>
    [JsonPropertyName("legal_representative")]
    public string? LegalRepresentative { get; set; }

    /// <summary>获取或设置企业名称（官方 <c>enterprise_name</c>）。</summary>
    [JsonPropertyName("enterprise_name")]
    public string? EnterpriseName { get; set; }

    /// <summary>获取或设置组成形式（官方 <c>type_of_organization</c>）。</summary>
    [JsonPropertyName("type_of_organization")]
    public string? TypeOfOrganization { get; set; }

    /// <summary>获取或设置经营场所 / 企业住所（官方 <c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>获取或设置公司类型（官方 <c>type_of_enterprise</c>）。</summary>
    [JsonPropertyName("type_of_enterprise")]
    public string? TypeOfEnterprise { get; set; }

    /// <summary>获取或设置经营范围（官方 <c>business_scope</c>）。</summary>
    [JsonPropertyName("business_scope")]
    public string? BusinessScope { get; set; }

    /// <summary>获取或设置注册资本（官方 <c>registered_capital</c>）。</summary>
    [JsonPropertyName("registered_capital")]
    public string? RegisteredCapital { get; set; }

    /// <summary>获取或设置实收资本（官方 <c>paid_in_capital</c>）。</summary>
    [JsonPropertyName("paid_in_capital")]
    public string? PaidInCapital { get; set; }

    /// <summary>获取或设置营业期限（官方 <c>valid_period</c>）。</summary>
    [JsonPropertyName("valid_period")]
    public string? ValidPeriod { get; set; }

    /// <summary>获取或设置注册日期 / 成立日期（官方 <c>registered_date</c>）。</summary>
    [JsonPropertyName("registered_date")]
    public string? RegisteredDate { get; set; }

    /// <summary>获取或设置营业执照位置（官方 <c>cert_position</c>）。</summary>
    [JsonPropertyName("cert_position")]
    public MpOcrRect? CertPosition { get; set; }

    /// <summary>获取或设置图片尺寸（官方 <c>img_size</c>）。</summary>
    [JsonPropertyName("img_size")]
    public MpImageSize? ImageSize { get; set; }
}

/// <summary>
/// 通用印刷体识别（<c>POST /cv/ocr/comm</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>items</c>（识别结果）/ <c>img_size</c>（图片大小）；
/// 条目字段表仅列 <c>pos</c>（位置信息）。
/// </para>
/// <para>
/// <b>官方文档缺陷（照录）</b>：返回示例中 <c>items[]</c> 含 <c>text</c>（识别文本），
/// 但「RES.ITEMS(ARRAY) OBJECT PAYLOAD」字段表只列了 <c>pos</c>；SDK 按超集建模保留
/// <see cref="MpOcrTextItem.Text"/>。
/// </para>
/// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c> / <c>101002</c> / <c>101003</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrCommResponse : MpResponse
{
    /// <summary>获取或设置识别结果列表（官方 <c>items</c>）。</summary>
    [JsonPropertyName("items")]
    public List<MpOcrTextItem>? Items { get; set; }

    /// <summary>获取或设置图片尺寸（官方 <c>img_size</c>）。</summary>
    [JsonPropertyName("img_size")]
    public MpImageSize? ImageSize { get; set; }
}

/// <summary>通用印刷体识别条目（官方 <c>items[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrTextItem
{
    /// <summary>获取或设置位置信息（官方 <c>pos</c>）。</summary>
    [JsonPropertyName("pos")]
    public MpOcrRect? Position { get; set; }

    /// <summary>获取或设置识别文本（官方返回示例字段 <c>text</c>；官方字段表未收录，照录）。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

/// <summary>
/// 菜单识别（<c>POST /cv/ocr/menu</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>content</c>（识别的信息）为 object，内含
/// <c>menu_items</c>（菜单内容列表，条目字段 <c>name</c> 菜单名 / <c>price</c> 价格）。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录，SDK 以字段表为口径）</b>：官方返回示例把 <c>content</c> 写成 JSON <b>字符串</b>
/// （需二次解析），与字段表标注的 object 不一致；SDK 按字段表建模为对象
/// （与素材域「字段表与正文不一致时以字段表为口径」的既存裁决同源）。若实测为字符串，宿主需自行二次解析。
/// </para>
/// <para>
/// <b>本页无频率上限</b>（官方全页无频次章节，且原文「该接口尚未接入服务平台，暂时不支持付费购买」）
/// ——SDK 不编造数值。错误码：<c>-1</c> / <c>101000</c> / <c>101001</c> / <c>101002</c>
/// （本页错误码表未列 <c>40001</c>，照录）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrMenuResponse : MpResponse
{
    /// <summary>获取或设置识别的信息（官方 <c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public MpOcrMenuContent? Content { get; set; }
}

/// <summary>菜单识别信息（官方 <c>content</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrMenuContent
{
    /// <summary>获取或设置菜单内容列表（官方 <c>menu_items</c>）。</summary>
    [JsonPropertyName("menu_items")]
    public List<MpOcrMenuItem>? MenuItems { get; set; }
}

/// <summary>菜单条目（官方 <c>content.menu_items[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrMenuItem
{
    /// <summary>获取或设置菜单名（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置价格（官方 <c>price</c>）。</summary>
    [JsonPropertyName("price")]
    public double? Price { get; set; }
}
