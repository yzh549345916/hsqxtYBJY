   M S B A M L   `   `   `  ����k  g呼和浩特市精细化天气预报评分系统, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulls>clr-namespace:呼和浩特市精细化天气预报评分系统0呼和浩特市精细化天气预报评分系统  5      L    F呼和浩特市精细化天气预报评分系统.预报考核查询页   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Plocal>clr-namespace:呼和浩特市精细化天气预报评分系统     5        N� Title$  预报考核查询页��5	      $	��550��6   $	��850��6%    � 	Resources 5         contentCenterStyle �� 
TargetType �� Property �� Value  DataGridStyle1&   System.Windows.Controls.DataGrid �� 
TargetType �� Padding J� SnapsToDevicePixels ��Name	 � ColumnDefinitions
 � RowDefinitions   SelectAllCommand �� 
ResourceId �� TypeInTargetAssembly7  1System.Windows.Controls.DataGridHeadersVisibility
   All �� ConverterParameter!   HeadersVisibilityConverter �� 	Converter �� RelativeSource �� AncestorTypeG  ASystem.Windows.Controls.Primitives.DataGridColumnHeadersPresenter  Name   Column ��Name �� CanContentScroll ��Name �� ScrollableHeight � Maximum �� Orientation* �� #ComputedVerticalScrollBarVisibility �� ViewportHeight �� ViewportSize � Value  �� Mode! �� ScrollableWidth," �� %ComputedHorizontalScrollBarVisibility# �� ViewportWidth$ �� Triggers% ]� 
Conditions& �� Property' �� Value��%�  &	        &	 T     �� 5   
   " ��5      .�5      ��  � ��$ Center=�5      5      �� 5   
   "  6'   .�5      ��  � ��# � ��6+   �� 5       � ��# � ��6+   �� 5       � �� �	#FF688CAF6,   �� 5       � ��$ 1q�60   �� 5        �@ RowDetailsVisibilityMode$ VisibleWhenSelected=�69   �� 5       � �� . 6>   �� 5       �@��PanningMode$
 Both=�69   �� 5       �@��IsFlicksEnabled .  67   �� 5       � �� 5      �� 5      "  6&   .R�5      �� #��z��6!   #��z��6M   #��z��6�   # z��6�    . ._�5      �� $ DG_ScrollViewer��6+   |�.  $��13(�6V   ��5   "   �� 5    &   " ��66   .R�5!   *   � 	 5"   .   �� 5#   2   $
��Auto �6C   �� 5$   2   $��* �6C   �� 5%   2   $
��Auto �6C   
 5'   .   �� 5(   2   $
��Auto �6@   �� 5)   2   $��* �6@   �� 5*   2   $
��Auto �6@   .+�5,   .   �� #��Z 65   |�.  ��6v   C� *�� $" DataGridSelectAllButtonStyle��"  +y�6�   �� *HeadersVisibility+# Z # Z  �� "  ��6�  �� *CellsPanelHorizontalOffset+ �� "    5-   .   $! PART_ColumnHeadersPresenter��6M   $��1��6r   y�6�   �� *HeadersVisibility+# Z # Z  �� "  �� 5.   .   $! PART_ScrollContentPresenter��6E   # z��6j   $��2��6�   $��1��6�   �� 5/   .   $ PART_VerticalScrollBar��68   $��2��6X   # z 6h   $ Vertical=�6�   $��1��6�   #y�z 6�   # z 6[   6�   �� *VerticalOffset+$  OneWay=� �� *TemplatedParent+6[  � 50   .   $��1��63   $��2��6C   	 51   2   �� 52   6   ��6G   �� **(NonFrozenColumnsViewportHorizontalOffset+ �� "  �� 53   6   $��* �6G   .+�55   2   �� $ PART_HorizontalScrollBar��6<   $��1��6^   # z! 6n   $ 
Horizontal=�6�   #y�z" 6�   # z# 6Y   6�   �� *HorizontalOffset+$  OneWay=� �� *TemplatedParent+6Y  56   /   57   +   58   '   .��5:   "   �� # z 61   5;      5<      5=      5?      $ 5@      ]� 5A      % 5B      �� 5C      & �@��
IsGrouping' . 6:   ./�5E      ��  � �� .  6F   5F      5H      5I      .��5J      � $��0,0,0,0q�6   .+�5K   
     -   5        (  Name$( JTPFList��5K      $
��Left=�6>   $	��282��6Y   $��
13,100,0,0q�6f   $	��Top=�6z   )   AutoGenerateColumns) .  $��Center=�6�   "*   VerticalScrollBarVisibility$* Visible=�6�   $��14(�6�   +   
IsReadOnly+ . ���	#FF1654AE#���  6/  $��Center=�6X  ��6&   �� ,   ColumnHeaderStyle, 5L      �� 5M      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  5N      .�5O      ��  � ��$ Center=�5P   ?   5Q      -   Columns- 5V      0  *System.Windows.Controls.DataGridTextColumn  5W      ,  &System.Windows.Controls.DataGridColumn.   Header$. 姓名��6%   /   Width5  /System.Windows.Controls.DataGridLengthConverter$/ 80 6J   1  +System.Windows.Controls.DataGridBoundColumn0   ElementStyle#0 [  6U   1   FontSize$1 16Q�6�   2   Binding2 61   �� *Name+6�     5X      $. ID��6%   $1 18Q�6H   $/ 90 6V   #0 [  6a   2 61   �� *ID+6a     5Y      $. 站点总数��6%   $1 18Q�6L   $/ 70 6Z   #0 [  6e   2 63   �� *ZDZS+6e     5Z      $. 高温1��6%   $1 18Q�6J   $/ 70 6X   #0 [  6c   2 62   �� *GW1+6c     5[      $. 高温2��6%   $1 18Q�6J   $/ 70 6X   #0 [  6c   2 62   �� *GW2+6c     5\      $. 高温3��6%   $1 18Q�6J   $/ 70 6X   #0 [  6c   2 62   �� *GW3+6c     5]      $. 低温1��6%   $1 18Q�6J   $/ 70 6X   #0 [  6c   2 62   �� *DW1+6c     5^      $. 低温2��6%   $1 18Q�6J   $/ 70 6X   #0 [  6c   2 62   �� *DW2+6c     5_      $. 低温3��6%   $1 18Q�6J   $/ 70 6X   #0 [  6c   2 62   �� *DW3+6c     5`      $. 晴雨1��6%   $1 18Q�6J   $/ 70 6X   #0 [  6c   2 62   �� *QY1+6c     5a      $. 晴雨2��6%   $1 18Q�6J   $/ 70 6X   #0 [  6c   2 62   �� *QY2+6c   5d      (	  "System.Windows.Controls.DatePicker	  5e   
   -   5        3 	 Name$3 sDate��5e      $
��Left=�6%   $��	46,28,0,0q�6@   $	��Top=�6S   $��28��6k   $	��130��6w   
���   �
�������4 	  IsTodayHighlighted4 . 5 	  SelectedDateFormat$
5 Long=�6�   6�   	  5f   
   -   5        $3 eDate��5f      $
��Left=�6$   $��
250,28,0,0q�6?   $	��Top=�6S   $��28��6k   $	��130��6w   
���   �
�������4 .  $
5 Long=�6�   �� 5g   
   -   5        6 ��Name$6 labelStartdate��5g      $��开始日期��6(   $
��Left=�67   $��	78,-3,0,0q�6R   $	��Top=�6e   $��26��6}   $��58��6�   7 J� RenderTransformOrigin$7 
-2.466,0.5&�6�   �� 5h   
   -   5        $6 labelEnddate��5h      $��结束日期��6&   $
��Left=�65   $��
287,-3,0,0q�6P   $	��Top=�6d   $7 0.362,3&�6|   $��26��6�   $��58��6�   �� 5i   
   -   5        $6 BTLabel��5i      $�� ��6!   $
��Left=�6,   $��	59,60,0,0q�6G   $	��Top=�6Z   $��Center=�6r   $��Center=�6�   $	��659��6�   $��20(�6�   $��楷体)�6�   �� 5j   
   -   5        8 ��Name$8 CXButton��5j      $��查询��6#   $
��Left=�60   $��
459,26,0,0q�6K   $	��Top=�6_   $	��110��6w   $��27��6�   ���	#FF5BABEE
�������6�   �� 5k   
   -   5        $8 DCButton��5k      $��导出��6#   $
��Left=�60   $��
630,26,0,0q�6K   $	��Top=�6_   $	��110��6w   $��27��6�   ���	#FF5BABEE
�������6�   �� 5l   
   -	   5        $6 SMLabel��5l      $�� ��6!   $
��Left=�6,   $��
48,387,0,0q�6G   $	��Top=�6[   $��Center=�6s   $��Center=�6�   $	��659��6�   $��20(�6�   $��楷体)�6�   $	��153��6�   5n      5o      