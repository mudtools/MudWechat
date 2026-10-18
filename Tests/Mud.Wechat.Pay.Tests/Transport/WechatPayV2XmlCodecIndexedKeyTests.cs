// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.V2.DataModels.PayV2;
using Mud.Wechat.Pay.V2.Transport;

namespace Mud.Wechat.Pay.Tests.Transport;

/// <summary>
/// APIv2 XML 编解码对<b>下标形态应答键</b>（<c>customs_$n</c> / <c>detail_$n</c>）的覆盖证明。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么要专门测这一条</b>：报关查询官方把多海关申报的逐条结果编成带下标的键
/// （第 1 个海关的结果在 <c>customs_1</c>，第 2 个在 <c>customs_2</c>……），
/// 而 <see cref="WechatPayV2XmlCodec"/> 的中间形态是<b>扁平字段表</b> ⇒
/// 若应答 DTO 只有固定属性，这些键会在「字段表 → 属性」这一步被<b>静默丢弃</b>：
/// 既不报错、也不留痕，症状是「拆单申报永远只读到一条」。
/// </para>
/// <para>
/// <see cref="V2CustomsQueryResponse"/> 用 <c>[JsonExtensionData]</c> 承接它们，本文件即该设计的
/// 行为证明 —— 同时钉住「数值汇总字段仍按 DTO 类型还原」这条既有口径不被扩展袋破坏。
/// </para>
/// </remarks>
public class WechatPayV2XmlCodecIndexedKeyTests
{
    private static readonly WechatPayV2ContentSerializer Serializer = new();

    /// <summary>下标键进袋、固定键进属性，且 <c>count</c> 仍还原成数值。</summary>
    [Fact]
    public void Deserialize_ShouldKeepIndexedKeys_WhenResponseUsesNumberedFieldNames()
    {
        var xml =
            "<xml>"
            + "<return_code><![CDATA[SUCCESS]]></return_code>"
            + "<return_msg><![CDATA[ok]]></return_msg>"
            + "<result_code><![CDATA[SUCCESS]]></result_code>"
            + "<mch_id><![CDATA[1900000000]]></mch_id>"
            + "<transaction_id><![CDATA[4200000000202610180000000001]]></transaction_id>"
            + "<count><![CDATA[2]]></count>"
            + "<verify_department><![CDATA[海关总署]]></verify_department>"
            + "<customs><![CDATA[shenzhen]]></customs>"
            + "<customs_state><![CDATA[declared]]></customs_state>"
            + "<customs_1><![CDATA[GUANGZHOU]]></customs_1>"
            + "<customs_state_1><![CDATA[waiting]]></customs_state_1>"
            + "<customs_2><![CDATA[SHENZHEN]]></customs_2>"
            + "<customs_state_2><![CDATA[declared]]></customs_state_2>"
            + "</xml>";

        var response = Serializer.Deserialize<V2CustomsQueryResponse>(xml);

        response.Should().NotBeNull();
        response!.Count.Should().Be(2,
            "because count 在 DTO 上声明为 int?，扁平表全字符串的形态必须按目标类型还原（否则数值字段一律读成 0 或直接抛）");
        response.VerifyDepartment.Should().Be("海关总署");

        // 袋里只应剩「既非固定属性、又确实在报文里」的键。
        response.IndexedFields.Should().NotBeNull();
        response.IndexedFields!.Keys.Should().Contain(new[] { "customs_1", "customs_state_1", "customs_2", "customs_state_2" },
            "because 多海关申报的逐条结果只有下标键；丢掉任一条即商户读到残缺状态");
        response.IndexedFields!["customs_1"].GetString().Should().Be("GUANGZHOU");
    }

    /// <summary>固定属性不得同时出现在扩展袋里（否则一条契约两处真相）。</summary>
    [Fact]
    public void Deserialize_ShouldNotDuplicateFixedKeys_WhenExtensionDataIsPresent()
    {
        var xml =
            "<xml><return_code>SUCCESS</return_code><count>1</count>"
            + "<verify_department>GZ</verify_department><verify_department_trade_id>ABC</verify_department_trade_id>"
            + "<transaction_id>T1</transaction_id><mch_id>1900000000</mch_id></xml>";

        var response = Serializer.Deserialize<V2CustomsQueryResponse>(xml);

        response!.IndexedFields.Should().BeNull("because STJ 只把「无匹配属性」的键放进袋里");
        response.TransactionId.Should().Be("T1");
        response.VerifyDepartmentTradeId.Should().Be("ABC");
    }

    /// <summary>无 <c>sign</c> 与非 XML 报文仍按既有口径读成空表 / default，不在解码层抛。</summary>
    [Fact]
    public void Deserialize_ShouldReturnDefault_WhenPayloadIsNotXml()
    {
        Serializer.Deserialize<V2CustomsQueryResponse>("<html>参数格式错误</html>")
            .Should().BeNull(
                "because 官方在参数错误场景返回 HTML；解码层抛异常会丢掉原文，排障无从下手 ⇒ 交回调用方按 return_code 缺失判失败");
    }
}
