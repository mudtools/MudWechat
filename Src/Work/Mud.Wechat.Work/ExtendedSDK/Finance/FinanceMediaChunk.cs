// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.ExtendedSDK.Finance;

/// <summary>
/// <c>GetMediaData</c> 的<b>单个分片</b>结果（不可变快照）。
/// </summary>
/// <remarks>
/// <para>
/// 官方把媒体按分片回传（单片上限 512KB），因此「一片」是本域最小可理解单位：
/// 需要边下边落盘的宿主直接用 <see cref="IWechatWorkFinanceClient.GetMediaChunkAsync"/> 逐片写文件；
/// 需要完整字节的宿主用 <see cref="IWechatWorkFinanceClient.GetMediaDataAsync"/>（内部按
/// <see cref="NextIndexBuffer"/> 续传、以 <see cref="IsFinished"/> 收尾）。
/// </para>
/// <para>
/// <b><see cref="NextIndexBuffer"/> 为 <c>null</c> 只出现在末片</b>（或原生未回写游标）。
/// 若在 <see cref="IsFinished"/> 为假时拿到空游标，聚合器会抛错而不是「从头再来」——
/// 从头再来等于把同一文件重复拼接到自己前面（守卫 FIN-B3 锁定该判据）。
/// </para>
/// </remarks>
public sealed class FinanceMediaChunk
{
    /// <summary>构造分片快照（仅本封装内部使用）。</summary>
    /// <param name="content">本片字节。</param>
    /// <param name="nextIndexBuffer">续传游标（<c>null</c> = 原生未回写）。</param>
    /// <param name="isFinished">是否已到末片。</param>
    internal FinanceMediaChunk(byte[] content, string? nextIndexBuffer, bool isFinished)
    {
        Content = content;
        NextIndexBuffer = nextIndexBuffer;
        IsFinished = isFinished;
    }

    /// <summary>本片字节。</summary>
    public byte[] Content { get; }

    /// <summary>本片字节数。</summary>
    public int Length => Content.Length;

    /// <summary>续传游标（送下一次 <c>GetMediaData</c> 的 <c>indexbuf</c>）。</summary>
    public string? NextIndexBuffer { get; }

    /// <summary>原生 <c>IsMediaDataFinish</c> 非 0 判定结果：本片是否为末片。</summary>
    public bool IsFinished { get; }
}

/// <summary>
/// 分片聚合器 —— 「按游标续传 + 以完成标记收尾 + 同游标退避重试」的纯逻辑，与原生调用解耦。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么单独成类</b>：聚合正确性（多片拼接顺序、游标推进、终止条件、重试不推进游标、零进度死循环）
/// 是本域最容易写错、又最难在真机上验证的一段。把它做成接受「取片委托」的纯函数，守卫 FIN-B3 就能用
/// 假分片源逐项断言（含 512KB 边界与「已完成但未回游标」的异常形态），而不需要原生库在场。
/// </para>
/// <para>
/// <b>总量上限取 <see cref="int"/> 数组极限</b>（而非新增配置项）：返回值是 <c>byte[]</c>，
/// CLR 单数组本就不能超过 <c>int.MaxValue</c>；超限即抛并引导改用逐片接口落盘 ——
/// 这样「边界」由返回类型自然给出，不引入一个可能与真实极限漂移的配置面。
/// </para>
/// </remarks>
internal static class FinanceMediaAssembler
{
    /// <summary>聚合字节数上限（CLR 单数组极限留余量）。</summary>
    internal const int MaxAggregateBytes = int.MaxValue - 1024;

    /// <summary>
    /// 聚合全部分片。
    /// </summary>
    /// <param name="fetchShard">取一片（入参为续传游标，首片为 <c>null</c>）。</param>
    /// <param name="retryCount">同游标重试次数（0 = 关闭）。</param>
    /// <param name="retryDelayMs">重试退避间隔（毫秒）。</param>
    /// <param name="cancellationToken">取消令牌（只在分片边界生效，见下）。</param>
    /// <returns>拼接后的完整字节。</returns>
    /// <remarks>
    /// <b>取消的诚实边界</b>：令牌只在「取下一片之前」与「退避等待中」被检查 —— 已进入的原生调用是阻塞的、
    /// 不可中断。把这一点写出来是为了不让调用方以为 <c>cancel</c> 能让一次正在进行的下载立刻返回。
    /// </remarks>
    internal static async Task<byte[]> AggregateAsync(
        Func<string?, Task<FinanceMediaChunk>> fetchShard,
        int retryCount,
        int retryDelayMs,
        CancellationToken cancellationToken)
    {
        if (fetchShard == null) throw new ArgumentNullException(nameof(fetchShard));
        if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount));

        var segments = new List<byte[]>();
        long total = 0;
        string? cursor = null;
        var attempts = 0;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            FinanceMediaChunk chunk;
            try
            {
                chunk = await fetchShard(cursor).ConfigureAwait(false);
            }
            catch (WechatFinanceNativeException ex) when (attempts < retryCount && WechatFinanceNativeCodes.IsMediaShardRetryable(ex.ReturnCode))
            {
                // 重试**同一游标**：推进游标等于跳过失败的那一片，拼出来的文件是残缺的。
                attempts++;
                if (retryDelayMs > 0)
                {
                    await Task.Delay(retryDelayMs, cancellationToken).ConfigureAwait(false);
                }

                continue;
            }

            attempts = 0;

            if (chunk.Length > 0)
            {
                total += chunk.Length;
                if (total > MaxAggregateBytes)
                {
                    throw new InvalidOperationException(
                        $"媒体聚合字节数超过 {MaxAggregateBytes}（CLR 单数组上限），无法整体返回。" +
                        "请改用 GetMediaChunkAsync 逐片落盘。");
                }

                segments.Add(chunk.Content);
            }

            if (chunk.IsFinished)
            {
                break;
            }

            if (string.IsNullOrEmpty(chunk.NextIndexBuffer))
            {
                // 未完成却没有游标：继续只能是「用同一个游标再取一次」，那会无限重复同一片。
                throw new InvalidOperationException(
                    "媒体分片未结束但原生未回写续传游标（indexbuf 为空）。已聚合 " + total + " 字节，中止聚合以避免重复拼接。");
            }

            if (chunk.Length == 0)
            {
                // 有游标却零字节 —— 官方语义下这是「原地不动」，逐片循环会永不收敛。
                throw new InvalidOperationException(
                    "媒体分片返回 0 字节且未标记完成，判定为原生侧无进度，中止聚合。");
            }

            cursor = chunk.NextIndexBuffer;
        }

        var result = new byte[total];
        var offset = 0;
        foreach (var segment in segments)
        {
            Buffer.BlockCopy(segment, 0, result, offset, segment.Length);
            offset += segment.Length;
        }

        return result;
    }
}
