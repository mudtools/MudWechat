// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

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
