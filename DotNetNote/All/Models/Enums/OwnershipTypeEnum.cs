using System;

namespace All.Models.Enums
{
    public enum OwnershipTypeEnum
    {
        Employees, // 기본값(우선순위 높음)
        Vendors
    }

    public static class OwnershipTypeHelper
    {
        /// <summary>
        /// 문자열 값을 OwnershipTypeEnum으로 변환(기본값: Employees)
        /// </summary>
        public static OwnershipTypeEnum Parse(string value)
        {
            if (Enum.TryParse(value, true, out OwnershipTypeEnum result))
            {
                return result;
            }
            return OwnershipTypeEnum.Employees; // 기본값 Employees
        }

        /// <summary>
        /// OwnershipTypeEnum을 문자열로 변환
        /// </summary>
        public static string ToStringValue(this OwnershipTypeEnum ownershipType)
        {
            return ownershipType.ToString();
        }
    }
}
