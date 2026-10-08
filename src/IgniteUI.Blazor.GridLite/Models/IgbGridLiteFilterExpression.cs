using System.Text.Json.Serialization;
using IgniteUI.Blazor.Controls.Internal;

namespace IgniteUI.Blazor.Controls;

/// <summary>
/// Represents a filter operation for a given column.
/// </summary>
public class IgbGridLiteFilterExpression
{
    /// <summary>
    /// The target column for the filter operation.
    /// </summary>
    [JsonPropertyName("key")]
    public required string Key { get; set; }

    /// <summary>
    /// The name of the filter condition to apply. The valid names depend on the column's
    /// <see cref="IgbGridLiteColumn.DataType"/>; a column without one counts as <see cref="GridLiteColumnDataType.String"/>.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader><term>Data type</term><description>Condition names</description></listheader>
    /// <item>
    /// <term><see cref="GridLiteColumnDataType.String"/></term>
    /// <description><c>contains</c>, <c>doesNotContain</c>, <c>startsWith</c>, <c>endsWith</c>, <c>equals</c>,
    /// <c>doesNotEqual</c>, <c>empty</c>, <c>notEmpty</c></description>
    /// </item>
    /// <item>
    /// <term><see cref="GridLiteColumnDataType.Number"/></term>
    /// <description><c>equals</c>, <c>doesNotEqual</c>, <c>greaterThan</c>, <c>lessThan</c>, <c>greaterThanOrEqual</c>,
    /// <c>lessThanOrEqual</c>, <c>empty</c>, <c>notEmpty</c></description>
    /// </item>
    /// <item>
    /// <term><see cref="GridLiteColumnDataType.Boolean"/></term>
    /// <description><c>all</c>, <c>true</c>, <c>false</c>, <c>empty</c>, <c>notEmpty</c></description>
    /// </item>
    /// </list>
    /// <para>
    /// The boolean conditions, <c>empty</c> and <c>notEmpty</c> are unary: they take no <see cref="SearchTerm"/>.
    /// </para>
    /// </remarks>
    [JsonPropertyName("condition")]
    public required string Condition { get; set; }

    /// <summary>
    /// The filtering value used in the filter condition function.
    /// Optional for unary conditions.
    /// </summary>
    /// <remarks>
    /// Typically matches the <see cref="IgbGridLiteColumn.DataType"/> holding a <see cref="string"/>,
    /// a <see cref="double"/> (any number, including one set as an <see cref="int"/>) or a <see cref="bool"/>;
    /// falls back to <see cref="System.Text.Json.JsonElement"/> for other unrecognized/app-provided types.
    /// </remarks>
    [JsonPropertyName("searchTerm")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? SearchTerm { get; set; }

    /// <summary>
    /// How this expression resolves in relation to the column's other expressions.
    /// </summary>
    [JsonPropertyName("criteria")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GridLiteFilterCriteria? Criteria { get; set; }

    /// <summary>
    /// Whether the filter operation should be case sensitive.
    /// If not provided, the value is resolved based on the column filter configuration.
    /// </summary>
    [JsonPropertyName("caseSensitive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? CaseSensitive { get; set; }
}

/// <summary>
/// How a filter expression resolves in relation to the column's other expressions.
/// </summary>
[JsonConverter(typeof(CamelCaseEnumConverter<GridLiteFilterCriteria>))]
public enum GridLiteFilterCriteria
{
    /// <summary>The record must pass all the conditions.</summary>
    And,

    /// <summary>The record must pass at least one condition.</summary>
    Or,
}
