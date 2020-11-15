using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.Runtime;


namespace SendEmail2SelectedGroup 
{
  #region BooleanToVisibilityConverter --------------------------------------------------------------------
  // There is a System.Windows.Controls.BooleanToVisibilityConverter too.

  public class BooleanToVisibilityCollapseConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Boolean)
      {
        return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
      }

      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class BooleanToVisibilityHiddenConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Boolean)
      {
        return ((bool)value) ? Visibility.Visible : Visibility.Hidden;
      }

      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class BooleanToVisibilityInverseCollapseConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Boolean)
      {
        return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
      }

      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class BooleanToVisibilityInverseHiddenConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Boolean)
      {
        return ((bool)value) ? Visibility.Hidden : Visibility.Visible;
      }

      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  /*
   * <Window x:Class="OviAdmKIR.ParameterWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:OwnConv="clr-namespace:eMeL.WPF;assembly=EMELCommonClassWPFLibrary"
        ...
  <StackPanel>
    <StackPanel.Resources>
        <OwnConv:BooleanToVisibilityHiddenConverter x:Key="boolToVisibilityHidden" />
    </StackPanel.Resources>
 
    <CheckBox x:Name="chkShowDetails" Content="Show Details" />
    <StackPanel x:Name="detailsPanel" Visibility="{Binding IsChecked, ElementName=chkShowDetails, Converter={StaticResource boolToVisibilityHidden}}">
    </StackPanel>
  </StackPanel>
  */
  #endregion BooleanToVisibilityConverter

  #region BooleanNegationConverter ------------------------------------------------------------------------

  public class BooleanNegationConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Boolean)
      {
        return ! ((bool)value);
      }

      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Boolean)
      {
        return !((bool)value);
      }

      return value;
    }
  }

  /*
   <Window x:Class="OviAdmKIR.ParameterWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:OwnConv="clr-namespace:eMeL.WPF;assembly=EMELCommonClassWPFLibrary"
   ...
 
  <StackPanel>
    <StackPanel.Resources>
        <OwnConv:BooleanNegationConverter x:Key="boolNegation" />
    </StackPanel.Resources>
 
    <CheckBox x:Name="chkShowDetails" Content="Show Details" />
    
    <TextBox IsReadOnly="{Binding IsChecked, ElementName=chkShowDetails, Converter={StaticResource boolNegation}}"
  </StackPanel>
  */
  #endregion BooleanToVisibilityConverter

  #region CloneConverter ----------------------------------------------------------------------------------

  // http://blogs.msdn.com/b/mikehillberg/archive/2006/09/26/cannotanimateimmutableobjectinstance.aspx
  // prevent exception: "Cannot animate '...' on an immutable object instance"

  public class CloneConverter : IValueConverter
  {
    public static CloneConverter Instance = new CloneConverter();
 
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Freezable)
        {
            value = (value as Freezable).Clone();
        }
 
        return value;
    }
 
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
  }
  #endregion CloneConverter

  #region InvertedBooleanToVisibilityConverter ------------------------------------------------------------
  /// <summary>
  /// Represents the converter that converts Boolean values to and from <see cref="T:System.Windows.Visibility"/> enumeration values.
  /// </summary>
  [Localizability(LocalizationCategory.NeverLocalize)]
  public sealed class InvertedBooleanToVisibilityConverter : IValueConverter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Controls.BooleanToVisibilityConverter"/> class.
    /// </summary>
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public InvertedBooleanToVisibilityConverter()
    {
    }

    /// <summary>
    /// Converts a Boolean value to a <see cref="T:System.Windows.Visibility"/> enumeration value.
    /// </summary>
    /// 
    /// <returns>
    /// <see cref="F:System.Windows.Visibility.Visible"/> if <paramref name="value"/> is true; otherwise, <see cref="F:System.Windows.Visibility.Collapsed"/>.
    /// </returns>
    /// <param name="value">The Boolean value to convert. This value can be a standard Boolean value or a nullable Boolean value.</param><param name="targetType">This parameter is not used.</param><param name="parameter">This parameter is not used.</param><param name="culture">This parameter is not used.</param>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool flag = false;

      if (value is bool)
        flag = (bool)value;

      return (Visibility)(flag ? 2 : 0);
    }

    /// <summary>
    /// Converts a <see cref="T:System.Windows.Visibility"/> enumeration value to a Boolean value.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="value"/> is <see cref="F:System.Windows.Visibility.Visible"/>; otherwise, false.
    /// </returns>
    /// <param name="value">A <see cref="T:System.Windows.Visibility"/> enumeration value. </param><param name="targetType">This parameter is not used.</param><param name="parameter">This parameter is not used.</param><param name="culture">This parameter is not used.</param>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Visibility)
        return (Visibility)value != Visibility.Visible;
      else
        return false;
    }            
  }
  #endregion InvertedBooleanToVisibilityConverter

  #region NullToBooleanConverter --------------------------------------------------------------------------

  [ValueConversion(typeof(object), typeof(bool))]
  public class NullToBooleanConverter : System.Windows.Data.IValueConverter
  { // http://anders.janmyr.com/2007/06/nulltobooleanconverter-for-wpf.html
    // http://stackoverflow.com/questions/15357234/null-to-boolean-ivalueconverter-not-working
      
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (targetType != typeof(bool))
      {
        throw new InvalidOperationException("NullToBooleanConverter/Convert: parameter típusa kötelezően bool!");    
      }

      return (value != null);        
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  /*
    <Window x:Class="......"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:myns="clr-namespace:RomvariSzakdolgozat"     
        <!-- xmlns:OwnConv="clr-namespace:.....;assembly=......." -->

  <StackPanel>
    <StackPanel.Resources>
        <myns:NullToBooleanConverter x:Key="nullToBool" />
    </StackPanel.Resources>
   
    <TextBox IsReadOnly="{Binding anyViewModelObject, Converter={StaticResource nullToBool}}"
  </StackPanel>
  */
  #endregion NullToBooleanConverter
}

