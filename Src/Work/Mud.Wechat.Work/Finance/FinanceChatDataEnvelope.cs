// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Finance;

/// <summary>
/// 会话内容存档 C SDK <c>GetChatData</c> 写回 <c>Slice_t</c> 的密文信封
/// （<c>{"errcode":0,"errmsg":"ok","chatdata":[...]}</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>这不是 HTTP 响应</b>：该 JSON 由原生库在进程内写入，因此它复用企微统一信封的 <c>errcode</c>/<c>errmsg</c>
/// 形状（与 <c>/cgi-bin/*</c> 同形），判错走 <c>WechatWorkException.ThrowIfFailed</c> 同一咽喉点。
/// </para>
/// <para>
/// <b>两层判错不可混为一谈</b>：<c>GetChatData</c> 的<b>原生返回码</b>（0/10001/10002/…）与这里的
/// <c>errcode</c> 是两层事实 —— 原生返回码非 0 时 <c>Slice</c> 内容不保证是合法 JSON，
/// 反之原生返回 0 而 <c>errcode != 0</c> 也可能出现（如密钥/机器人权限问题）。
/// </para>
/// <para>
/// <b>字段来源</b>：与本地 SKIT 源码
/// <c>SKIT.FlurlHttpClient.Wechat.Work/ExtendedSDK/Finance/Models/GetChatRecordsResponse.cs</c> 对齐；
/// 官方页 <c>https://developer.work.weixin.qq.com/document/path/91774</c> 的逐字段原文**待逐页核验**。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Finance")]
public class FinanceChatDataEnvelope : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置本次拉取到的会话记录列表（官方键 <c>chatdata</c>）。
    /// </summary>
    /// <remarks>
    /// <b>可为 <c>null</c></b>：官方在无新数据时是否省略该键还是回空数组，未在已核验材料中出现 ⇒
    /// 调用侧必须区分「空列表」与「字段缺省」，不得把 <c>null</c> 当 0 条以外的含义。
    /// </remarks>
    [JsonPropertyName("chatdata")]
    public List<FinanceChatDataRow>? ChatData { get; set; }
}

/// <summary>
/// 会话记录的一行密文（<see cref="FinanceChatDataEnvelope.ChatData"/> 的元素）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本类型全程只承载密文</b>：<see cref="EncryptRandomKey"/> 与 <see cref="EncryptChatMsg"/> 都是密文串，
/// 任何情况下不得写入日志、遥测或异常消息（守卫 FIN-B5 锁定）。
/// </para>
/// <para>
/// <b>解密时序由官方固定</b>：先以 <see cref="PublicKeyVersion"/> 找到对应 RSA 私钥，
/// 对 <see cref="EncryptRandomKey"/> 做 base64 解码 + RSA(PKCS#1) 解密得到 <c>encrypt_key</c> 明文，
/// 再把 <c>encrypt_key</c> 与 <see cref="EncryptChatMsg"/> 一起送 <c>DecryptData</c>。
/// 顺序颠倒或跳过版本匹配必然失败（官方支持密钥轮换，故版本映射是必填链路而非优化项）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Finance")]
public class FinanceChatDataRow
{
    /// <summary>
    /// 获取或设置序列号（官方键 <c>seq</c>）—— 下次拉取的游标。
    /// </summary>
    /// <remarks>
    /// 官方为 <c>uint64</c>，故 CLR 取 <see cref="ulong"/>；分页语义是「带上上次拉到的<b>最大</b> seq 继续」。
    /// </remarks>
    [JsonPropertyName("seq")]
    public ulong Seq { get; set; }

    /// <summary>
    /// 获取或设置消息 ID（官方键 <c>msgid</c>）。
    /// </summary>
    [JsonPropertyName("msgid")]
    public string? MessageId { get; set; }

    /// <summary>
    /// 获取或设置加密该条记录随机密钥所用的公钥版本号（官方键 <c>publickey_ver</c>）。
    /// </summary>
    /// <remarks>
    /// 官方支持公钥/私钥多版本并存（轮换），因此该值是<b>私钥映射的键</b>而非展示信息。
    /// </remarks>
    [JsonPropertyName("publickey_ver")]
    public int PublicKeyVersion { get; set; }

    /// <summary>
    /// 获取或设置随机密钥的密文（官方键 <c>encrypt_random_key</c>：RSA PKCS#1 加密后再 base64）。
    /// </summary>
    [JsonPropertyName("encrypt_random_key")]
    public string? EncryptRandomKey { get; set; }

    /// <summary>
    /// 获取或设置会话内容的密文（官方键 <c>encrypt_chat_msg</c>，与 AES key 一同送 <c>DecryptData</c>）。
    /// </summary>
    [JsonPropertyName("encrypt_chat_msg")]
    public string? EncryptChatMsg { get; set; }
}
