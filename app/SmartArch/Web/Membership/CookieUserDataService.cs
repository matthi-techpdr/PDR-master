using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartArch.Web.Membership
{
    /// <summary>
    /// Represents service for storing and selecting user data in cookie.
    /// </summary>
    public class CookieUserDataService : ICookieUserDataService
    {
        /// <summary>
        /// The separator between elements in user data.
        /// </summary>
        private const string DATA_SEPARATOR = "|";

        /// <summary>
        /// The wrapper for area name in user data.
        /// </summary>
        private const string AREA_WRAPPER = "$";

        /// <summary>
        /// The name of licensee area of user data.
        /// </summary>
        private const string ID_AREA_NAME = "Id";
        
        /// <summary>
        /// Creates the user data for storing in cookie.
        /// </summary>
        /// <param name="user">The membership user.</param>
        /// <returns>The user data as string.</returns>
        public string CreateUserData(IMembershipUser user)
        {
            // Check.Require<ArgumentNullException>(user != null, "Unable convert user to string for store as UserData of form authenticate ticket because user is null");

            string userData = string.Empty;
            // added user id
            var userIdData = CreateUserDataArea(new List<string> { user.Id.ToString() }, ID_AREA_NAME);
            userData += userIdData;

            return userData;
        }

        /// <summary>
        /// Gets the id of user.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <returns>The id of user.</returns>
        public string GetId(string userData)
        {
            // Check.Require<ArgumentNullException>(userData != null, "Unable gets id from user data if it is null");

            var id = GetAreaContent(userData, ID_AREA_NAME).SingleOrDefault() ?? string.Empty;

            return id;
        }

        /// <summary>
        /// Creates the user data area.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="areaName">Name of the area.</param>
        /// <returns>The user data.</returns>
        private static string CreateUserDataArea(IEnumerable<string> source, string areaName)
        {
            string areaContent = string.Join(DATA_SEPARATOR, source);
            string areaKey = GetDataAreaKey(areaName);
            string areaData = areaKey + areaContent;

            return areaData;
        }

        /// <summary>
        /// Gets the data area key.
        /// </summary>
        /// <param name="areaName">Name of the area.</param>
        /// <returns>The key of area of data. It is start position of area data.</returns>
        private static string GetDataAreaKey(string areaName)
        {
            string wrappedAreaName = AREA_WRAPPER + areaName + AREA_WRAPPER;

            return wrappedAreaName;
        }

        /// <summary>
        /// Gets the content of the area of user data.
        /// </summary>
        /// <param name="data">The user data as string.</param>
        /// <param name="areaName">Name of the area.</param>
        /// <returns>The content of specified area of data.</returns>
        private static IEnumerable<string> GetAreaContent(string data, string areaName)
        {
            string areaKey = GetDataAreaKey(areaName);
            int areaKeyStartIndex = data.IndexOf(areaKey);
            string areaContent = string.Empty;
            if (areaKeyStartIndex != -1)
            {
                int areaContentStartIndex = areaKeyStartIndex + areaKey.Length;
                int areaContentEndIndex = data.IndexOf(AREA_WRAPPER, areaContentStartIndex);
                if (areaContentEndIndex == -1)
                {
                    areaContentEndIndex = data.Length;
                }

                int areaContentLength = areaContentEndIndex - areaContentStartIndex;

                areaContent = data.Substring(areaContentStartIndex, areaContentLength);
            }

            var areaContentList = areaContent.Split(new[] { DATA_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);

            return areaContentList;
        }
    }
}