   M S B A M L   `   `   `  ����G  CMusic Player, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:Music_PlayerMusic_Player  5          Music_Player.App   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ,localclr-namespace:Music_Player     5        �� 	Resources  5         SliderRepeatButton �� 
TargetType �� Property �� Value �� 
TargetType  SliderRepeatButton1 J� SnapsToDevicePixels  SliderThumb 
 Slider � RowDefinitions ]�Name ]� DecreaseRepeatButton	 ]� Thumb
 ]� IncreaseRepeatButton  Horizontal_Slider �� Triggers P� Property P� Value��%�  &	        &	 d    &	 �    &	 �    7  7  7   &	 7    7  �� 5   
   " ��6+   .�5      ��  �@J�SnapsToDevicePixels . 64   �� 5	       �@�OverridesDefaultStyle . 66   �� 5
       � �� .  6*   �� 5       � |� .  6*   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� $��1q�6"   
�������
�������$��4��6]   5      5      5      �� 5   
   " ��6,   .�5      ��  �@J�SnapsToDevicePixels . 64   �� 5       �@�OverridesDefaultStyle . 66   �� 5       � �� 5      �� 5      " ��6&   .R�5      ��  . 
���  ��$��1q�6N   
���  ��$��5��6t   5      5      5      �� 5!   
   " n�6$   .�5"      ��  �@J�SnapsToDevicePixels . 64   �� 5#       �@�OverridesDefaultStyle . 66   �� 5$       � �� 5%      �� 5&      " n�6&   .R�5'      A� $��15��6"   $��15��6.   
���  ��6F   5(      5*      5+      �� 5-   
   " ��6*   .R�5.      �  5/      �� 50      $
��Auto �6$   �� 51      $
��Auto �6$   #��z��62   �� 52      $
��Auto �6$   .+�54      ]� $ 
PART_Track��6%   $��1��6    55      �� 56      8��  $��Slider.DecreaseLarge��6U   	 58      n� 59      8�� 6    
 5;      �� 5<      8�� $��Slider.IncreaseLarge��6S   5>      5?      5@      �� 5B   
   " ��6*   .�5C      ��  � |� .  6*   �� 5D       �@J�SnapsToDevicePixels . 64   �� 5E       �@�OverridesDefaultStyle . 66    5F      P� 5G       �@��Orientation$ 
Horizontal=�61   . �5H      ��  � ��$ 21��62   �� 5I       � ��$	 104��61   �� 5J       � ��8   61   5K      5M      5N      5O      